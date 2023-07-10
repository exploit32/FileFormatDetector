using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace Tests.TextFiles
{
    internal class TextTestsHelper
    {
        internal static Encoding UTF32BE => Encoding.GetEncoding(12001);
        internal static Encoding Windows1251 => Encoding.GetEncoding(1251);

        static TextTestsHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        internal static async Task<DetectableEncoding?> EncodeAndDetect(string text, Encoding encoding, byte[]? bom = null)
        {
            var bytes = encoding.GetBytes(text);

            NonBomEncodingDetector detector = new NonBomEncodingDetector();

            DetectableEncoding? detectedEncoding;

            if (bom != null)
            {
                byte[] newBuffer = new byte[bytes.Length + bom.Length];

                Array.Copy(bom, newBuffer, bom.Length);
                Array.Copy(bytes, 0, newBuffer, bom.Length, bytes.Length);

                bytes = newBuffer;
            }

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                detectedEncoding = await detector.TryDetectEncoding(stream, null, CancellationToken.None);
            }

            return detectedEncoding;
        }

        internal static async Task<FormatSummary?> EncodeAndDetectFull(string text, Encoding encoding, byte[]? bom = null)
        {
            var bytes = encoding.GetBytes(text);

            TextFilesDetector detector = new TextFilesDetector();

            FormatSummary? detectedEncoding;

            if (bom != null)
            {
                byte[] newBuffer = new byte[bytes.Length + bom.Length];

                Array.Copy(bom, newBuffer, bom.Length);
                Array.Copy(bytes, 0, newBuffer, bom.Length, bytes.Length);

                bytes = newBuffer;
            }

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                detectedEncoding = await detector.ReadFormat(stream, null, CancellationToken.None);
            }

            return detectedEncoding;
        }

        internal static async Task<FormatSummary?> EncodeAndDetectFull(byte[] encodedText, byte[] bom)
        {
            TextFilesDetector detector = new TextFilesDetector();

            FormatSummary? detectedEncoding;

            byte[] newBuffer = new byte[encodedText.Length + bom.Length];

            Array.Copy(bom, newBuffer, bom.Length);
            Array.Copy(encodedText, 0, newBuffer, bom.Length, encodedText.Length);

            using (MemoryStream stream = new MemoryStream(newBuffer))
            {
                detectedEncoding = await detector.ReadFormat(stream, null, CancellationToken.None);
            }

            return detectedEncoding;
        }

        public static void CheckFormat(DetectableEncoding encoding, bool hasBom, FormatSummary format)
        {
            Assert.IsType<TextFormatSummary>(format);
            var textFormat = format as TextFormatSummary;
            Assert.NotNull(textFormat);

            Assert.Equal(encoding.CodePage, textFormat.CodePage);
            Assert.Equal(encoding.Name, textFormat.EncodingName);
            Assert.Equal(encoding.DisplayName, textFormat.EncodingFullName);
            Assert.Equal(hasBom, textFormat.HasBOM);
        }
    }
}
