using FormatApi;
using System.Text;

namespace BomTextFilesFormat
{
    public class BomTextFilesDetector : ITextFotmatDetector
    {
        private static Signature Utf8 = new Signature(new byte[] { 0xEF, 0xBB, 0xBF });
        private static Signature Utf16BE = new Signature(new byte[] { 0xFE, 0xFF });
        private static Signature Utf16LE = new Signature(new byte[] { 0xFF, 0xFE });
        private static Signature Utf32BE = new Signature(new byte[] { 0x0, 0x0, 0xFE, 0xFF });
        private static Signature Utf32LE = new Signature(new byte[] { 0xFF, 0xFE, 0x0, 0x0 });
        private static Signature Utf7 = new Signature(new byte[] { 0x2B, 0x2F, 0x76 });
        private static Signature Utf1 = new Signature(new byte[] { 0xF7, 0x64, 0x4C });
        private static Signature UtfEBCDIC = new Signature(new byte[] { 0xDD, 0x73, 0x66, 0x73 });
        private static Signature SCSU = new Signature(new byte[] { 0x0E, 0xFE, 0xFF });
        private static Signature BOCU1 = new Signature(new byte[] { 0xFB, 0xEE, 0x28 });
        private static Signature GB18030 = new Signature(new byte[] { 0x84, 0x31, 0x95, 0x33 });

        private static EncodingWithSignature[] Encodings = new EncodingWithSignature[]
        {
            new EncodingWithSignature(new Signature(Encoding.UTF8.GetPreamble()),  Encoding.UTF8.CodePage),
        };

        public FormatSummary? ReadFormat(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}