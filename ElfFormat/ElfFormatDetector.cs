using FormatApi;
using Tools;

namespace ElfFormat
{
    /// <summary>
    /// Detector of ELF format files
    /// </summary>
    public class ElfFormatDetector : IFormatDetector
    {
        public static readonly Signature _signature = new Signature(new byte[] { 0x7F, (byte)'E', (byte)'L', (byte)'F' });

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
        public string Description => "ELF format detector";

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

            ElfFormatReader elfReader = new ElfFormatReader(reader);

            var elfHeader = elfReader.ReadHeader();

            var programHeaderSegments = elfReader.ReadProgramHeaders(elfHeader);

            var interpreterSegment = programHeaderSegments.FirstOrDefault(s => s.Type == Structs.SegmentType.Interpreter);

            string interpreterName = interpreterSegment != null ? elfReader.ReadInterpreterSegment(interpreterSegment) : string.Empty;

            FormatSummary summary = new ElfFormatSummary(            
                architecture: elfHeader.Machine.ToString(),
                bits: elfHeader.Class == Structs.ElfClass.Bit32 ? 32 : 64,
                endianness: elfHeader.Endianness == Structs.ElfEndianness.LittleEndian ? Endianness.LittleEndian : Endianness.BigEndian,
                interpreter: interpreterName
            );

            return Task.FromResult<FormatSummary?>(summary);
        }
    }
}