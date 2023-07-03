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

        public NonBomEncodingDetector(bool nullSuggestsBinary)
        {
            _nullSuggestsBinary = nullSuggestsBinary;
        }

        public DetectableEncoding? TryDetermineEncoding(Stream stream, long totalFileSize)
        {
            FileSummary summary = CalculateSummary(stream);

            bool sizeIsMultipleOf4 = totalFileSize % 4 == 0;
            bool sizeIsMultipleOf2 = totalFileSize % 2 == 0;

            if (summary.ContainsNulls)
            {
                //Possible: Utf-16, Utf-32
                if (sizeIsMultipleOf4 && summary.ContainsNullTriples && summary.SequentialNulls <= 3)
                {
                    //Possible: Utf-32
                    if (summary.LittleEndianNullTriples > summary.BigEndianNullTriples)
                        return DetectableEncoding.Utf32LE;
                    else
                        return DetectableEncoding.Utf32BE;
                }

                if (sizeIsMultipleOf2 && summary.SequentialNulls <= 1)
                {
                    if (summary.NullsAt0Position + summary.NullsAt2Position > summary.NullsAt1Position + summary.NullsAt3Position)
                        return DetectableEncoding.Utf16BE;
                    else
                        return DetectableEncoding.Utf16LE;
                }
            }
            else
            {
                //Utf8, ASCII, ANSI
                if (summary.OnlyAsciiRange)
                    return DetectableEncoding.ASCII;
                else if (summary.ValidUtf8Sequences)
                    return DetectableEncoding.Utf8;
                else
                    return DetectableEncoding.ISO8859;
            }

            return null;
        }

        private FileSummary CalculateSummary(Stream stream)
        {
            long maxSequentialNulls = 0;
            long sequentialNulls = 0;

            bool readSuccess = true;
            bool onlyAsciiCharacter = true;
            
            byte[] buffer = new byte[4];

            int utf8ExtraBytes = 0;
            bool notUtf8 = false;
            long nullsAt0 = 0;
            long nullsAt1 = 0;
            long nullsAt2 = 0;
            long nullsAt3 = 0;
            long tripleNullBe = 0;
            long tripleNullLe = 0;
            bool validUtf16LeSurrogates = true;
            bool validUtf16BeSurrogates = true;

            while (readSuccess)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead < buffer.Length)
                    break;


                for (int i = 0; i < bytesRead; i++)
                {
                    byte ch = buffer[i];

                    if (ch == 0)
                    {
                        sequentialNulls++;

                        if (sequentialNulls > maxSequentialNulls)
                            maxSequentialNulls = sequentialNulls;
                    }
                    else
                    {
                        sequentialNulls = 0;
                    }

                    // Utf8 sequences validation
                    if (!notUtf8)
                    {
                        if (utf8ExtraBytes == 0)
                        {
                            if (ch <= 127)
                            {
                                // 1 byte
                            }
                            else if (ch >= 194 && ch <= 223)
                            {
                                // 2 Byte
                                utf8ExtraBytes = 1;
                            }
                            else if (ch >= 224 && ch <= 239)
                            {
                                // 3 Byte
                                utf8ExtraBytes = 2;
                            }
                            else if (ch >= 240 && ch <= 244)
                            {
                                // 4 Byte
                                utf8ExtraBytes = 3;
                            }
                            else
                            {
                                notUtf8 = true;
                                onlyAsciiCharacter = false;
                            }
                        }
                        else
                        {
                            onlyAsciiCharacter = false;
                            if (ch < 128 || ch > 191)
                            {
                                notUtf8 = true;
                            }
                            --utf8ExtraBytes;
                        }
                    }
                }


                if (buffer[0] == 0)
                    nullsAt0++;

                if (buffer[1] == 0)
                    nullsAt1++;

                if (buffer[2] == 0)
                    nullsAt2++;

                if (buffer[3] == 0)
                    nullsAt3++;

                if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0 && buffer[3] != 0)
                    tripleNullBe++;

                if (buffer[0] != 0 && buffer[1] == 0 && buffer[2] == 0 && buffer[3] == 0)
                    tripleNullLe++;
                
            }

            return new FileSummary()
            {
                OnlyAsciiRange = onlyAsciiCharacter,
                NullsAt0Position = nullsAt0,
                NullsAt1Position = nullsAt1,
                NullsAt2Position = nullsAt2,
                NullsAt3Position = nullsAt3,
                ValidUtf8Sequences = !notUtf8,
                LittleEndianNullTriples = tripleNullLe,
                BigEndianNullTriples = tripleNullBe,
                SequentialNulls = maxSequentialNulls,
            };
        }

        public DetectableEncoding? CheckUtf8(Stream stream)
        {
            // UTF8 Valid sequences
            // 0xxxxxxx  ASCII
            // 110xxxxx 10xxxxxx  2-byte
            // 1110xxxx 10xxxxxx 10xxxxxx  3-byte
            // 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx  4-byte
            //
            // Width in UTF8
            // Decimal      Width
            // 0-127        1 byte
            // 194-223      2 bytes
            // 224-239      3 bytes
            // 240-244      4 bytes
            //
            // Subsequent chars are in the range 128-191
            var onlySawAsciiRange = true;
            bool readSuccess = true;

            while (readSuccess)
            {
                int readResult = stream.ReadByte();
                readSuccess = readResult != -1;

                if (!readSuccess)
                    break;

                byte ch = (byte)readResult;

                if (ch == 0 && _nullSuggestsBinary)
                {
                    return null;
                }

                int moreChars;
                if (ch <= 127)
                {
                    // 1 byte
                    moreChars = 0;
                }
                else if (ch >= 194 && ch <= 223)
                {
                    // 2 Byte
                    moreChars = 1;
                }
                else if (ch >= 224 && ch <= 239)
                {
                    // 3 Byte
                    moreChars = 2;
                }
                else if (ch >= 240 && ch <= 244)
                {
                    // 4 Byte
                    moreChars = 3;
                }
                else
                {
                    return null; // Not utf8
                }

                // Check secondary chars are in range if we are expecting any
                while (moreChars > 0)
                {
                    onlySawAsciiRange = false; // Seen non-ascii chars now

                    //ch = buffer[pos++];
                    int nextChar = stream.ReadByte();

                    readSuccess = nextChar != -1;

                    if (!readSuccess)
                        break;
                    else
                        ch = (byte)nextChar;

                    if (ch < 128 || ch > 191)
                    {
                        return null; // Not utf8
                    }

                    --moreChars;
                }
            }

            // If we get to here then only valid UTF-8 sequences have been processed

            // If we only saw chars in the range 0-127 then we can't assume UTF8 (the caller will need to decide)
            return onlySawAsciiRange ? DetectableEncoding.ASCII : DetectableEncoding.Utf8;
        }
    }
}
