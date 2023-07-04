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
            EvenOddNullsCalculator evenOddNullsCalculator = new EvenOddNullsCalculator();
            SequentialNullsCalculator sequentialNullsCalculator = new SequentialNullsCalculator();
            Utf32Checker utf32Checker = new Utf32Checker();
            BytesRangeCalculator bytesRangeCalculator = new BytesRangeCalculator();

            bool mayBeAscii = true;
            bool mayBeUtf8 = true;
            bool mayBeUtf16 = stream.Length % 2 == 0;
            bool mayBeUtf32 = stream.Length % 4 == 0;

            while(stream.Position < readLength)
            {
                int bytesToRead = (int)Math.Min(readLength - stream.Position, bufferSize);

                int bytesRead = stream.Read(buffer, 0, bytesToRead);

                ReadOnlySpan<byte> bytes = buffer.AsSpan(0, bytesRead);

                if (mayBeAscii)
                    mayBeAscii = bytesRangeCalculator.AllBytesLessThan128(bytes);

                if (mayBeUtf8)
                    mayBeUtf8 = utf8Checker.ProcessBlock(bytes);

                if (mayBeUtf16)
                    evenOddNullsCalculator.ProcessBlock(bytes);

                if (mayBeUtf32)
                    utf32Checker.ProcessBlock(bytes);

                sequentialNullsCalculator.ProcessBlock(bytes);
            }

            if (evenOddNullsCalculator.ContainsNulls)
            {
                //Possible: Utf-16, Utf-32
                if (mayBeUtf32 && utf32Checker.HasNullTriples && sequentialNullsCalculator.MaxSequentialNulls <= 3)
                {
                    //Possible: Utf-32
                    if (utf32Checker.LittleEndianSymbols > utf32Checker.BigEndianSymbols)
                        return DetectableEncoding.Utf32LE;
                    else
                        return DetectableEncoding.Utf32BE;
                }

                if (mayBeUtf16 && sequentialNullsCalculator.MaxSequentialNulls <= 1)
                {
                    if (evenOddNullsCalculator.EvenNulls > evenOddNullsCalculator.OddNulls)
                        return DetectableEncoding.Utf16BE;
                    else
                        return DetectableEncoding.Utf16LE;
                }
            }
            else
            {
                //Utf8, ASCII, ANSI
                if (mayBeAscii)
                    return DetectableEncoding.ASCII;
                else if (mayBeUtf8)
                    return DetectableEncoding.Utf8;
                else
                    return DetectableEncoding.ISO8859;
            }

            return null;
        }
    }
}
