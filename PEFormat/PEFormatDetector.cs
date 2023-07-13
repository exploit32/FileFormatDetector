using FormatApi;
using PEFormat.Structs;
using Tools;

namespace PEFormat
{
    public class PEFormatDetector : IBinaryFormatDetector
    {
        public static readonly Signature Signature = new Signature(new byte[] { (byte)'M', (byte)'Z' });

        public bool HasSignature => true;

        public int BytesToReadSignature => Signature.Offset + Signature.Value.Length;

        public string Description => "PE format";

        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            if (fileStart.Length < BytesToReadSignature)
                return false;

            return SignatureTools.CheckSignature(fileStart, Signature);
        }

        public async Task<FormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken)
        {
            EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, true);

            PEFormatReader peReader = new PEFormatReader(reader);

            DosHeader dosHeader = peReader.ReadDosHeader();
            COFFHeader coffHeader = peReader.ReadCOFFHeader(dosHeader);
            OptionalHeader? optionalHeader = peReader.ReadOptionalHeader(coffHeader);

            int bits = GetBitsByOptionalHeader(optionalHeader);

            if (bits == 0)
                bits = coffHeader.GetBitsByMachineType();

            PEFormatSummary? formatSummary = new PEFormatSummary()
            {
                Architecture = coffHeader.Machine.ToString(),
                Bits = bits,
                Endianness = GetEndianess(coffHeader.Machine),
                HasClrHeader = HasClrMetadata(optionalHeader),
            };

            return formatSummary;
        }

        private Endianness GetEndianess(Machine machine)
        {
            return (ushort)machine == 0x01F2 ? Endianness.BigEndian : Endianness.LittleEndian; // IBM PowerPC Big-Endian (XBOX 360)
        }

        private bool HasClrMetadata(OptionalHeader? optionalHeader)
        {
            if (optionalHeader == null)
                return false;

            if (optionalHeader.DataDirectories.Length < (int)DataDirectoryType.ClrRuntime + 1)
                return false;

            DataDirectory clrDirectory = optionalHeader.DataDirectories[(int)DataDirectoryType.ClrRuntime];

            return clrDirectory.Size != 0 && clrDirectory.RVA != 0;
        }

        private int GetBitsByOptionalHeader(OptionalHeader? optionalHeader)
        {
            OptionalHeader.PEFormat? magic = optionalHeader?.Magic;
            
            return magic switch
            {
                null => 0,
                OptionalHeader.PEFormat.PE32 => 32,
                OptionalHeader.PEFormat.PE32Plus => 64,
                _ => throw new ArgumentException($"Value {magic} is unknown"),
            };
        }
    }
}