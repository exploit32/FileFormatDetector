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
    public class TextFilesDetector : ITextFormatDetector
    {
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
        .OrderByDescending(e => e.BomSignature.Value.Length)
        .ToArray();

        private Lazy<int> MinPreambleLength = new Lazy<int>(() => EncodingsWithSignature.Min(e => e.BomSignature.Value.Length + e.BomSignature.Offset));
        private Lazy<int> MaxPreambleLength = new Lazy<int>(() => EncodingsWithSignature.Max(e => e.BomSignature.Value.Length + e.BomSignature.Offset));

        public bool HasSignature => true;

        public FormatSummary? ReadFormat(Stream stream, long? maxBytesToRead)
        {
            FormatSummary? summary = null;

            try
            {
                byte[] buffer = new byte[MaxPreambleLength.Value];

                int bytesRead = stream.Read(buffer, 0, MaxPreambleLength.Value);

                if (bytesRead < MinPreambleLength.Value)
                    return null;

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
                else
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    summary = TryDetectEncodingWithoutBom(stream, maxBytesToRead);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return summary;
        }

        private DetectableEncoding? CheckSignatures(ReadOnlySpan<byte> fileStart, DetectableEncoding[] encodings)
        {
            for (int i = 0; i < encodings.Length; i++)
            {
                var encoding = encodings[i];

                if (fileStart.Length >= encoding.BomSignature.Offset + encoding.BomSignature.Value.Length)
                {
                    if (SignatureTools.CheckSignature(fileStart, encoding.BomSignature))
                        return encoding;
                }
            }

            return null;
        }

        private TextFormatSummary? TryDetectEncodingWithoutBom(Stream stream, long? maxBytesToRead)
        {
            TextFormatSummary? summary = null;

            NonBomEncodingDetector nonBomEncodingDetector = new NonBomEncodingDetector();

            var detectedEncoding = nonBomEncodingDetector.TryDetectEncoding(stream, maxBytesToRead);

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