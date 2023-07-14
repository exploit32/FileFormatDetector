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

        internal static async Task<FormatSummary?> EncodeAndDetectFull(string text, Encoding encoding, byte[]? bom = null, long? lengthLimit = null)
        {
            var bytes = encoding.GetBytes(text);

            return await EncodeAndDetectFull(bytes, bom, lengthLimit);
        }

        internal static async Task<FormatSummary?> EncodeAndDetectFull(byte[] encodedText, byte[]? bom = null, long? lengthLimit = null)
        {
            TextFilesDetector detector = new TextFilesDetector();
            detector.MaxBytesToRead = lengthLimit;

            FormatSummary? detectedEncoding;

            if (bom != null)
            {
                byte[] newBuffer = new byte[encodedText.Length + bom.Length];

                Array.Copy(bom, newBuffer, bom.Length);
                Array.Copy(encodedText, 0, newBuffer, bom.Length, encodedText.Length);

                encodedText = newBuffer;
            }

            using (MemoryStream stream = new MemoryStream(encodedText))
            {
                detectedEncoding = await detector.ReadFormat(stream, CancellationToken.None);
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
