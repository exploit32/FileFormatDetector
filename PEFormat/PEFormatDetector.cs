using FormatApi;
using PEFormat.Structs;
using Tools;

namespace PEFormat
{
    /// <summary>
    /// PE format files detector
    /// </summary>
    public class PEFormatDetector : IFormatDetector
    {
        public static readonly Signature _signature = new Signature(new byte[] { (byte)'M', (byte)'Z' });

        /// <summary>
        /// Indicates that format has signature
        /// </summary>
        public bool HasSignature => true;

        /// <summary>
        /// Indicates that signature is mandatory
        /// </summary>
        public bool SignatureIsMandatory => true;

        /// <summary>
        /// Number of bytes to read a signature
        /// </summary>
        public int BytesToReadSignature => _signature.Offset + _signature.Value.Length;

        /// <summary>
        /// Detector description
        /// </summary>
        public string Description => "PE format detector";

        /// <summary>
        /// Check if file contains signature
        /// </summary>
        /// <param name="fileStart">Beginning of the file</param>
        /// <returns>True is signature is found, otherwise False</returns>
        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            if (fileStart.Length < BytesToReadSignature)
                return false;

            return SignatureTools.CheckSignature(fileStart, _signature);
        }

        /// <summary>
        /// Read entire format
        /// </summary>
        /// <param name="stream">Opened file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Elf format summary</returns>
        /// <exception cref="FileFormatException">Exception is thrown if file is malformed</exception>
        public Task<FormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken)
        {
            EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, true);

            PEFormatReader peReader = new PEFormatReader(reader);

            DosHeader dosHeader = peReader.ReadDosHeader();
            COFFHeader coffHeader = peReader.ReadCOFFHeader(dosHeader);
            OptionalHeader? optionalHeader = peReader.ReadOptionalHeader(coffHeader);

            int bits = GetBitsByOptionalHeader(optionalHeader);

            if (bits == 0)
                bits = coffHeader.GetBitsByMachineType();

            PEFormatSummary? formatSummary = new PEFormatSummary(            
                architecture: coffHeader.Machine.ToString(),
                bits: bits,
                endianness: GetEndianess(coffHeader.Machine),
                hasClrHeader: HasClrMetadata(optionalHeader)
            );

            return Task.FromResult<FormatSummary?>(formatSummary);
        }

        private Endianness GetEndianess(Machine machine)
        {
            return machine == Machine.POWERPCBE ? Endianness.BigEndian : Endianness.LittleEndian; // IBM PowerPC Big-Endian (XBOX 360)
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