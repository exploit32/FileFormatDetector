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
        private static readonly DetectableEncoding[] _encodingsWithSignature = new DetectableEncoding[]
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

        private static readonly Lazy<int> _minBomLength = new Lazy<int>(() => _encodingsWithSignature.Min(e => e.BomSignature!.Value.Length + e.BomSignature.Offset));
        private static readonly Lazy<int> _maxBomLength = new Lazy<int>(() => _encodingsWithSignature.Max(e => e.BomSignature!.Value.Length + e.BomSignature.Offset));
        private long? _fileScanSizeLimit;

        /// <summary>
        /// Number of bytes to scan for probabilistic format detection. Null means no boundary
        /// </summary>
        [Parameter("file-scan-size-limit", "Number of bytes to scan for probabilistic format detection.\nMust be greater than 0 and be a multiple of 4.")]
        public long? FileScanSizeLimit
        {
            get => _fileScanSizeLimit;
            set
            {
                if (value.HasValue && value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(FileScanSizeLimit), "Value must be greater than 0");

                if (value.HasValue && value % 4 != 0)
                    throw new ArgumentException("Value must be a multiple of 4", nameof(FileScanSizeLimit));

                _fileScanSizeLimit = value;
            }
        }

        /// <summary>
        /// Indicates that format has signature
        /// </summary>
        public bool HasSignature => true;

        /// <summary>
        /// Indicates that signature is mandatory
        /// </summary>
        public bool SignatureIsMandatory => false;

        /// <summary>
        /// Number of bytes to read a signature
        /// </summary>
        public int BytesToReadSignature => _maxBomLength.Value;

        /// <summary>
        /// Detector description
        /// </summary>
        public string Description => "Text files detector. Supported encodings: ASCII, Windows-125x, UTF-8, UTF-16 BE/LE, UTF-32 BE/LE and BOM detection.";

        /// <summary>
        /// Check if file contains signature
        /// </summary>
        /// <param name="fileStart">Beginning of the file</param>
        /// <returns>True is signature is found, otherwise False</returns>
        public bool CheckSignature(ReadOnlySpan<byte> fileStart)
        {
            return CheckSignatures(fileStart, _encodingsWithSignature) != null;
        }

        /// <summary>
        /// Read entire format
        /// </summary>
        /// <param name="stream">Opened file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Elf format summary</returns>
        /// <exception cref="FileFormatException">Exception is thrown if file is malformed</exception>
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

        /// <summary>
        /// Check BOM signatures
        /// </summary>
        /// <param name="fileStart">Beginning of the file</param>
        /// <param name="encodings">Possible encodings</param>
        /// <returns>Encoding if BOMis found. Otherwise false</returns>
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

        /// <summary>
        /// Try to detect BOM header in file
        /// </summary>
        /// <param name="stream">Opened file</param>
        /// <returns>Format summary of BOM was found</returns>
        private async Task<TextFormatSummary?> TryDetectEncodingWithBom(Stream stream)
        {
            if (stream.Length < _minBomLength.Value)
                return null;

            byte[] buffer = new byte[_maxBomLength.Value];

            int bytesRead = await stream.ReadAsync(buffer, 0, _maxBomLength.Value);

            DetectableEncoding? detectedEncoding = CheckSignatures(buffer.AsSpan(0, bytesRead), _encodingsWithSignature);

            TextFormatSummary? summary = CreateFormatSummary(detectedEncoding, true);

            return summary;
        }

        /// <summary>
        /// Try to detect encoding for files without BOM
        /// </summary>
        /// <param name="stream">Opened file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Format summary if file is recognized. Otherwise null</returns>
        private async Task<TextFormatSummary?> TryDetectEncodingWithoutBom(Stream stream, CancellationToken cancellationToken)
        {
            NonBomEncodingDetector nonBomEncodingDetector = new NonBomEncodingDetector();

            var detectedEncoding = await nonBomEncodingDetector.TryDetectEncoding(stream, FileScanSizeLimit, cancellationToken);

            TextFormatSummary? summary = CreateFormatSummary(detectedEncoding, false);

            return summary;
        }

        private static TextFormatSummary? CreateFormatSummary(DetectableEncoding? detectedEncoding, bool hasBom)
        {
            TextFormatSummary? summary = null;

            if (detectedEncoding != null)
            {
                summary = new TextFormatSummary(
                    encodingName: detectedEncoding.Name,
                    encodingFullName: detectedEncoding.DisplayName,
                    codePage: detectedEncoding.CodePage,
                    hasBOM: hasBom
                );
            }

            return summary;
        }
    }
}