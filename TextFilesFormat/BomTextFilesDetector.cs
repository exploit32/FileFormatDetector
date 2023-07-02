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
        private const int MaxSizeToLoadInMemory = 16 * 1024 * 1024;

        private static EncodingWithSignature[] EncodingsWithSignature = new EncodingWithSignature[]
        {
            new EncodingWithSignature(new Signature(new byte[] { 0xEF, 0xBB, 0xBF }),       Encoding.UTF8.CodePage, "utf-8", "Unicode (UTF-8)"),
            new EncodingWithSignature(new Signature(new byte[] { 0xFF, 0xFE }),             Encoding.Unicode.CodePage, "utf-16LE", "Unicode (UTF-16 Little-Endian)"),
            new EncodingWithSignature(new Signature(new byte[] { 0xFE, 0xFF }),             Encoding.BigEndianUnicode.CodePage, "utf-16BE", "Unicode (UTF-16 Big-Endian)"),
            new EncodingWithSignature(new Signature(new byte[] { 0xFF, 0xFE, 0x0, 0x0 }),   Encoding.UTF32.CodePage, "utf-32LE", "Unicode (UTF-32 Little-Endian)"),
            new EncodingWithSignature(new Signature(new byte[] { 0x0, 0x0, 0xFE, 0xFF }),   12001, "utf-32BE", "Unicode (UTF-32 Big-Endian)"),
            new EncodingWithSignature(new Signature(new byte[] { 0x2B, 0x2F, 0x76 }),       65000, "utf-7", "Unicode (UTF-7)"),
            new EncodingWithSignature(new Signature(new byte[] { 0xF7, 0x64, 0x4C }),       0, "utf-1", "ISO-10646-UTF-1"),
            new EncodingWithSignature(new Signature(new byte[] { 0xDD, 0x73, 0x66, 0x73 }), 0, "utf-ebcdict", "utf-ebcdict"),
            new EncodingWithSignature(new Signature(new byte[] { 0x0E, 0xFE, 0xFF }),       0, "scsu", "Standard Compression Scheme for Unicode"),
            new EncodingWithSignature(new Signature(new byte[] { 0xFB, 0xEE, 0x28 }),       0, "bocu-1", "Binary Ordered Compression for Unicode"),
            new EncodingWithSignature(new Signature(new byte[] { 0x84, 0x31, 0x95, 0x33 }), 54936, "gb18030", "Chinese National Standard GB 18030-2005: Information Technology—Chinese coded character set"),
        }
        .OrderByDescending(e => e.Signature.Value.Length)
        .ToArray();

        private Lazy<int> MinPreambleLength = new Lazy<int>(() => EncodingsWithSignature.Min(e => e.Signature.Value.Length + e.Signature.Offset));
        private Lazy<int> MaxPreambleLength = new Lazy<int>(() => EncodingsWithSignature.Max(e => e.Signature.Value.Length + e.Signature.Offset));

        public bool HasSignature => true;

        public FormatSummary? ReadFormat(Stream stream, long? maxBytesToRead)
        {
            FormatSummary? summary = null;

            try
            {
                byte[] buffer = new byte[MaxPreambleLength.Value];

                int preambleLength = stream.Read(buffer, 0, MaxPreambleLength.Value);

                if (preambleLength < MinPreambleLength.Value)
                    return null;

                EncodingWithSignature? detectedEncoding = CheckSignatures(buffer.AsSpan(0, preambleLength), EncodingsWithSignature);

                if (detectedEncoding != null)
                {
                    summary = new TextFormatSummary()
                    {
                        EncodingName = detectedEncoding.EncodingName,
                        EncodingFullName = detectedEncoding.EncodingFullName,
                        CodePage = detectedEncoding.CodePage,
                        HasBOM = true,
                    };
                }
                else
                {
                    Stream streamToUse = stream;

                    if (maxBytesToRead.HasValue && maxBytesToRead < MaxSizeToLoadInMemory)
                    {
                        byte[] memoryBuffer = new byte[(int)maxBytesToRead.Value];
                        Array.Copy(buffer, memoryBuffer, preambleLength);
                        int additionalLength = stream.Read(memoryBuffer, preambleLength, (int)maxBytesToRead.Value - preambleLength);

                        MemoryStream memory = new MemoryStream(memoryBuffer, 0, additionalLength + preambleLength);

                        streamToUse = memory;
                    }
                    else
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    summary = TryDetectEncoding(streamToUse);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return summary;
        }

        private EncodingWithSignature? CheckSignatures(ReadOnlySpan<byte> fileStart, EncodingWithSignature[] signatures)
        {
            for (int i = 0; i < signatures.Length; i++)
            {
                var signature = signatures[i];

                if (fileStart.Length >= signature.Signature.Offset + signature.Signature.Value.Length)
                {
                    if (SignatureTools.CheckSignature(fileStart, signature.Signature))
                        return signature;
                }
            }

            return null;
        }

        private TextFormatSummary? TryDetectEncoding(Stream stream)
        {
            TextFormatSummary? summary = null;

            NonBomEncodingDetector nonBomEncodingDetector = new NonBomEncodingDetector(true);

            var detectedEncoding = nonBomEncodingDetector.CheckUtf8(stream);

            if (detectedEncoding == Encodings.Utf8Nobom)
            {
                summary = new TextFormatSummary()
                {
                    CodePage = Encoding.UTF8.CodePage,
                    EncodingName = "utf-8",
                    EncodingFullName = "Unicode (UTF-8)",
                    HasBOM = false,
                };
            }

            return summary;

        }
    }
}