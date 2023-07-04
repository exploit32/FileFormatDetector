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
        private bool _nullSuggestsBinary = true;

        private const int DefaultChunkSize = 4096;

        public NonBomEncodingDetector(bool nullSuggestsBinary)
        {
            _nullSuggestsBinary = nullSuggestsBinary;
        }

        public DetectableEncoding? TryDetermineEncoding(Stream stream, long? maxBytesToRead)
        {
            int bufferSize = maxBytesToRead.HasValue ? (int)Math.Min(maxBytesToRead.Value, DefaultChunkSize) : DefaultChunkSize;

            byte[] buffer = new byte[bufferSize];

             long readLength = maxBytesToRead.HasValue ? Math.Min(stream.Length, maxBytesToRead.Value) : stream.Length;

            Utf8Checker utf8Checker = new Utf8Checker();
            Utf16Checker utf16Checker = new Utf16Checker();
            Utf32Checker utf32Checker = new Utf32Checker();
            EvenOddNullsCalculator evenOddNullsCalculator = new EvenOddNullsCalculator();
            SequentialNullsCalculator sequentialNullsCalculator = new SequentialNullsCalculator();
            BytesRangeCalculator bytesRangeCalculator = new BytesRangeCalculator();

            bool mayBeUtf8 = true;
            bool mayBeUtf16 = stream.Length % 2 == 0;
            bool mayBeUtf32 = stream.Length % 4 == 0;
            bool mayBeAsciiOrWindows125x = true;

            while(stream.Position < readLength)
            {
                int bytesToRead = (int)Math.Min(readLength - stream.Position, bufferSize);

                int bytesRead = stream.Read(buffer, 0, bytesToRead);

                ReadOnlySpan<byte> bytes = buffer.AsSpan(0, bytesRead);

                if (mayBeAsciiOrWindows125x)
                    mayBeAsciiOrWindows125x = !bytesRangeCalculator.CheckControlChars(bytes);

                if (mayBeUtf8)
                    mayBeUtf8 = utf8Checker.CheckValidSurrogates(bytes);

                if (mayBeUtf16)
                {
                    evenOddNullsCalculator.ProcessBlock(bytes);
                    mayBeUtf16 = utf16Checker.CheckValidSurrogates(bytes);
                }

                if (mayBeUtf32)
                    mayBeUtf32 = utf32Checker.CheckValidRange(bytes);

                sequentialNullsCalculator.ProcessBlock(bytes);
            }


            //Possible: Utf-16, Utf-32
            if (mayBeUtf32 && sequentialNullsCalculator.MaxSequentialNulls <= 3)
            {
                //Possible: Utf-32
                if (utf32Checker.ValidLittleEndianRange)
                    return DetectableEncoding.Utf32LE;
                else if (utf32Checker.ValidBigEndianRange)
                    return DetectableEncoding.Utf32BE;
            }

            if (mayBeUtf16 && sequentialNullsCalculator.MaxSequentialNulls <= 1)
            {
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
                        if (evenOddNullsCalculator.EvenNulls > evenOddNullsCalculator.OddNulls)
                            return DetectableEncoding.Utf16BE;
                        else
                            return DetectableEncoding.Utf16LE;
                    }
                }
            }

            //Utf8, ASCII, ANSI
            if (!bytesRangeCalculator.HasAsciiControlChars)
                return DetectableEncoding.ASCII;
            else if (mayBeUtf8)
                return DetectableEncoding.Utf8;
            else if (!bytesRangeCalculator.HasWindows125xControlChars)
                return DetectableEncoding.Windows125x;


            return null;
        }
    }
}
