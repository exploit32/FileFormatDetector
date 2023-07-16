using FormatApi;
using System;
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
    public class TextFilesDetector : IFormatDetector
    {
        private static readonly DetectableEncoding[] EncodingsWithSignature = new DetectableEncoding[]
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

        private static readonly Lazy<int> MinBomLength = new Lazy<int>(() => EncodingsWithSignature.Min(e => e.BomSignature!.Value.Length + e.BomSignature.Offset));
        private static readonly Lazy<int> MaxBomLength = new Lazy<int>(() => EncodingsWithSignature.Max(e => e.BomSignature!.Value.Length + e.BomSignature.Offset));

        /// <summary>
        /// Number of bytes to scan for probabilistic format detection. Null means no boundary
        /// </summary>
        [Parameter("file-scan-size-limit", "Number of bytes to scan for probabilistic format detection.\nShould be greater than 0 and be a multiple of 4.")]
        public long? FileScanSizeLimit { get; set; }

        public bool HasSignature => true;

        public bool SignatureIsMandatory => false;

        public int BytesToReadSignature => MaxBomLength.Value;

        public string Description => "Text files detector. Supports BOM detection. Supported encodings: ASCII, UTF-8, UTF-16 BE/LE, UTF-32 BE/LE";

        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            return CheckSignatures(fileStart, EncodingsWithSignature) != null;
        }

        public async Task<FormatSummary?> ReadFormat(Stream stream, CancellationToken cancellationToken)
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

            if (stream.Length < MinBomLength.Value)
                return null;

            byte[] buffer = new byte[MaxBomLength.Value];

            int bytesRead = await stream.ReadAsync(buffer, 0, MaxBomLength.Value);

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

            var detectedEncoding = await nonBomEncodingDetector.TryDetectEncoding(stream, FileScanSizeLimit, cancellationToken);

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