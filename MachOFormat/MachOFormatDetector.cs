using FormatApi;
using MachOFormat.Structs;
using Tools;

namespace MachOFormat
{
    public class MachOFormatDetector : IFormatDetector
    {
        public static readonly Signature[] Signatures = new[]
        {
            MachOFormatReader.Magic,
            MachOFormatReader.Cigam,
            MachOFormatReader.Magic64,
            MachOFormatReader.Cigam64,
            MachOFormatReader.FatMagic,
            MachOFormatReader.FatMagic64,
        };

        public bool HasSignature => true;

        public bool SignatureIsMandatory => true;

        private readonly Lazy<int> _bytesToReadSignature = new Lazy<int>(() => Signatures.Max(s => s.Value.Length + s.Offset));

        public int BytesToReadSignature => _bytesToReadSignature.Value;

        public string Description => "Mach-O format detector";

        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            if (fileStart.Length < BytesToReadSignature)
                return false;

            return SignatureTools.CheckSignatures(fileStart, Signatures);
        }

        public async Task<FormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken)
        {
            EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, true);

            MachOFormatReader machOReader = new MachOFormatReader(reader);

            MachO machO = machOReader.ReadMachO();

            FormatSummary? summary = MakeSummary(machO);

            return summary;
        }

        private MachOFormatSummary MakeSummary(MachO machO)
        {
            return new MachOFormatSummary()
            {
                Bits = machO.Magic!.Bits,
                Endianness = machO.Magic.Endianness,
                IsFat = machO.Magic.IsFat,
                Architecture = machO.Architecture,
                HasSignature = machO.IsSigned,
                InnerApps = machO.InnerMachOs.Select(MakeSummary).ToArray(),
            };
        }
    }
}