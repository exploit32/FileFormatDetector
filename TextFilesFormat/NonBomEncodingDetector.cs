using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class NonBomEncodingDetector
    {
        private const int DefaultChunkSize = 4096;

        public async Task<DetectableEncoding?> TryDetectEncoding(Stream stream, long? maxBytesToRead)
        {
            if (maxBytesToRead.HasValue && maxBytesToRead % 4 != 0)
                throw new ArgumentException("Block size should be multiple of 4");

            int bufferSize = maxBytesToRead.HasValue ? (int)Math.Min(maxBytesToRead.Value, DefaultChunkSize) : DefaultChunkSize;

            byte[] buffer = new byte[bufferSize];

            long readLength = maxBytesToRead.HasValue ? Math.Min(stream.Length, maxBytesToRead.Value) : stream.Length;

            Utf8Checker utf8Checker = new Utf8Checker();
            Utf16Checker utf16Checker = new Utf16Checker();
            Utf32Checker utf32Checker = new Utf32Checker();
            AsciiChecker asciiChecker = new AsciiChecker();

            bool mayBeUtf8 = true;
            bool mayBeUtf16 = stream.Length % 2 == 0;
            bool mayBeUtf32 = stream.Length % 4 == 0;
            bool mayBeAsciiOrWindows125x = true;
            int bytesRead = 0;

            do
            {
                int bytesToRead = (int)Math.Min(readLength - stream.Position, bufferSize);
                bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);

                Memory<byte> bytes = buffer.AsMemory(0, bytesRead);

                if (mayBeAsciiOrWindows125x)
                    mayBeAsciiOrWindows125x = asciiChecker.CheckValidRange(bytes.Span);

                if (mayBeUtf8)
                    mayBeUtf8 = utf8Checker.CheckSurrogates(bytes.Span);

                if (mayBeUtf16)
                    mayBeUtf16 = utf16Checker.CheckValidRange(bytes.Span);

                if (mayBeUtf32)
                    mayBeUtf32 = utf32Checker.CheckValidRange(bytes.Span);

            } while (stream.Position < readLength && bytesRead > 0);

            //Assuming ASCII encoding if all bytes are less than 0x7F and there is no control chars lower than 0x20 except tab, carriage return, line feed
            if (asciiChecker.MayBeAscii)
                return DetectableEncoding.ASCII;

            //Assuming Utf8 if there is no nulls and utf8 surrogates were found
            if (mayBeUtf8 && utf8Checker.FoundSurrogates)
                return DetectableEncoding.Utf8;

            //Assuming Windown 1251 if there is no nulls and chars lower than 0x20 except tab, carriage return, line feed
            if (asciiChecker.MayBeWindows1251)
                return DetectableEncoding.Windows125x;

            if (mayBeUtf32)
            {
                //No NULL symbols, valid symbols value range =<10FFFF
                if (utf32Checker.ValidLittleEndianRange)
                    return DetectableEncoding.Utf32LE;
                else if (utf32Checker.ValidBigEndianRange)
                    return DetectableEncoding.Utf32BE;
            }

            if (mayBeUtf16)
            {
                //No Null symbols, no surrogate pairs value range violations
                if (utf16Checker.BigEndianSurrogatesValid && !utf16Checker.LittleEndianSurrogatesValid)
                    return DetectableEncoding.Utf16BE;
                else if (utf16Checker.LittleEndianSurrogatesValid && !utf16Checker.BigEndianSurrogatesValid)
                    return DetectableEncoding.Utf16LE;
                else
                {
                    if (utf16Checker.FoundBigEndianSurrogates && !utf16Checker.FoundLittleEndianSurrogates)
                        return DetectableEncoding.Utf16BE;
                    else if (utf16Checker.FoundLittleEndianSurrogates && !utf16Checker.FoundBigEndianSurrogates)
                        return DetectableEncoding.Utf16LE;
                    else
                    {
                        (int uniqueEvenBytes, int uniqueOddBytes) = utf16Checker.GetDistinctBytes();

                        if (uniqueEvenBytes > uniqueOddBytes)
                            return DetectableEncoding.Utf16LE;
                        else
                            return DetectableEncoding.Utf16BE;
                    }
                }
            }

            //Assuming Utf8 if there is no nulls and all utf8 surrogates are valid
            if (mayBeUtf8)
                return DetectableEncoding.Utf8;

            return null;
        }
    }
}
