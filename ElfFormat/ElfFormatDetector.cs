using FormatApi;
using Tools;

namespace ElfFormat
{
    public class ElfFormatDetector : IFormatDetector
    {
        public static readonly Signature Signature = new Signature(new byte[] { 0x7F, (byte)'E', (byte)'L', (byte)'F' });

        public bool HasSignature => true;

        public bool SignatureIsMandatory => true;

        public int BytesToReadSignature => Signature.Offset + Signature.Value.Length;

        public string Description => "ELF format detector";

        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            if (fileStart.Length < BytesToReadSignature)
                return false;

            return SignatureTools.CheckSignature(fileStart, Signature);
        }

        public async Task<FormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken)
        {
            EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, true);

            ElfFormatReader elfReader = new ElfFormatReader(reader);

            var elfHeader = elfReader.ReadHeader();

            var programHeaderSegments = elfReader.ReadProgramHeaders(elfHeader);

            var interpreterSegment = programHeaderSegments.FirstOrDefault(s => s.Type == Structs.SegmentType.Interpreter);

            string interpreterName = interpreterSegment != null ? elfReader.ReadInterpreterSegment(interpreterSegment) : string.Empty;

            FormatSummary? summary = new ElfFormatSummary()
            {
                Architecture = elfHeader.Machine.ToString(),
                Bits = elfHeader.Class == Structs.ElfClass.Bit32 ? 32 : 64,
                Endianness = elfHeader.Endianness == Structs.ElfEndianness.LittleEndian ? Endianness.LittleEndian : Endianness.BigEndian,
                Interpreter = interpreterName,
            };

            return summary;
        }
    }
}