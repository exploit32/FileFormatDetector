using FormatApi;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using Tools;

namespace TextFilesFormat
{
    /// <summary>
    /// Detector ot text files
    /// </summary>
    public class TextFilesDetector : ITextFormatDetector, IConfigurableDetector
    {
        private const string MaxBytesToReadParameterName = "max-bytes-to-read";

        private static ParameterDescription[] Parameters = new ParameterDescription[] { new ParameterDescription(MaxBytesToReadParameterName, "Detect format by first N bytes", false) };

        private static DetectableEncoding[] EncodingsWithSignature = new DetectableEncoding[]
        {
            DetectableEncoding.Utf1,
            DetectableEncoding.Utf7,
            DetectableEncoding.Utf8,
            DetectableEncoding.Utf16BE,
            DetectableEncoding.Utf16LE,
            DetectableEncoding.Utf32BE,
            DetectableEncoding.Utf32LE,
            DetectableEncoding.UtfEbcdict,
            DetectableEncoding.Bocu1,
            DetectableEncoding.Scsu,
            DetectableEncoding.Gb18030            
        }
        .Where(e => e.HasBomSignature)
        .OrderByDescending(e => e.BomSignature!.Value.Length)
        .ToArray();

        private static Lazy<int> MinPreambleLength = new Lazy<int>(() => EncodingsWithSignature.Min(e => e.BomSignature!.Value.Length + e.BomSignature.Offset));
        private static Lazy<int> MaxPreambleLength = new Lazy<int>(() => EncodingsWithSignature.Max(e => e.BomSignature!.Value.Length + e.BomSignature.Offset));

        /// <summary>
        /// Use only first N bytes to detect encoding
        /// </summary>
        public long? MaxBytesToRead { get; set; }

        public IEnumerable<ParameterDescription> GetParameters() => Parameters;

        public void SetParameter(string key, string value)
        {
            if (key == MaxBytesToReadParameterName)
            {
                if (long.TryParse(value, out var maxBytesToRead))
                {
                    if (maxBytesToRead <= 0)
                        throw new ArgumentException($"{key} must be greater than 0", key);

                    if (maxBytesToRead % 4 != 0)
                        throw new ArgumentException($"{key} value should be multiple of 4", key);

                    MaxBytesToRead = maxBytesToRead;
                }
                else
                    throw new ArgumentException($"Cannot parse {key} value {value}. Value should be positive long integer");
            }
        }


        public async Task<TextFormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken)
        {
            if (stream.Length == 0)
                return null;

            TextFormatSummary? summary = await TryDetectEncodingWithBom(stream);

            if (summary == null)
            {
                stream.Seek(0, SeekOrigin.Begin);
                summary = await TryDetectEncodingWithoutBom(stream, cancellationToken);
            }

            return summary;
        }


        private DetectableEncoding? CheckSignatures(ReadOnlySpan<byte> fileStart, DetectableEncoding[] encodings)
        {
            for (int i = 0; i < encodings.Length; i++)
            {
                var encoding = encodings[i];

                if (fileStart.Length >= encoding.BomSignature!.Offset + encoding.BomSignature.Value.Length)
                {
                    if (SignatureTools.CheckSignature(fileStart, encoding.BomSignature))
                        return encoding;
                }
            }

            return null;
        }

        private async Task<TextFormatSummary?> TryDetectEncodingWithBom(Stream stream)
        {
            TextFormatSummary? summary = null;

            if (stream.Length < MinPreambleLength.Value)
                return null;

            byte[] buffer = new byte[MaxPreambleLength.Value];

            int bytesRead = await stream.ReadAsync(buffer, 0, MaxPreambleLength.Value);

            DetectableEncoding? detectedEncoding = CheckSignatures(buffer.AsSpan(0, bytesRead), EncodingsWithSignature);

            if (detectedEncoding != null)
            {
                summary = new TextFormatSummary()
                {
                    EncodingName = detectedEncoding.Name,
                    EncodingFullName = detectedEncoding.DisplayName,
                    CodePage = detectedEncoding.CodePage,
                    HasBOM = true,
                };
            }

            return summary;
        }

        private async Task<TextFormatSummary?> TryDetectEncodingWithoutBom(Stream stream, CancellationToken cancellationToken)
        {
            TextFormatSummary? summary = null;

            NonBomEncodingDetector nonBomEncodingDetector = new NonBomEncodingDetector();

            var detectedEncoding = await nonBomEncodingDetector.TryDetectEncoding(stream, MaxBytesToRead, cancellationToken);

            if (detectedEncoding != null)
            {
                summary = new TextFormatSummary()
                {
                    CodePage = detectedEncoding.CodePage,
                    EncodingName = detectedEncoding.Name,
                    EncodingFullName = detectedEncoding.DisplayName,
                    HasBOM = false,
                };
            }

            return summary;
        }
    }
}