using FormatApi;
using MachOFormat.Structs;
using Tools;

namespace MachOFormat
{
    /// <summary>
    /// Mach-O file format detector
    /// </summary>
    public class MachOFormatDetector : IFormatDetector
    {
        public static readonly Signature[] _signatures = new[]
        {
            MachOFormatReader.Magic,
            MachOFormatReader.Cigam,
            MachOFormatReader.Magic64,
            MachOFormatReader.Cigam64,
            MachOFormatReader.FatMagic,
            MachOFormatReader.FatMagic64,
        };

        private readonly Lazy<int> _bytesToReadSignature = new Lazy<int>(() => _signatures.Max(s => s.Value.Length + s.Offset));

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
        public int BytesToReadSignature => _bytesToReadSignature.Value;

        /// <summary>
        /// Detector description
        /// </summary>
        public string Description => "Mach-O format detector";

        /// <summary>
        /// Check if file contains signature
        /// </summary>
        /// <param name="fileStart">Beginning of the file</param>
        /// <returns>True is signature is found, otherwise False</returns>
        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            if (fileStart.Length < BytesToReadSignature)
                return false;

            return SignatureTools.CheckSignatures(fileStart, _signatures);
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

            MachOFormatReader machOReader = new MachOFormatReader(reader);

            MachO machO = machOReader.ReadMachO();

            FormatSummary summary = MakeSummary(machO);

            return Task.FromResult<FormatSummary?>(summary);
        }

        private MachOFormatSummary MakeSummary(MachO machO)
        {
            return new MachOFormatSummary(
                architecture: machO.Architecture,
                bits: machO.Magic!.Bits,
                endianness: machO.Magic.Endianness,
                isFat: machO.Magic.IsFat,
                hasSignature: machO.IsSigned,
                innerApps: machO.InnerMachOs.Select(MakeSummary).ToArray()
            );
        }
    }
}