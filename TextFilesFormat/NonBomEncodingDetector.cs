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

        public Encodings CheckUtf8(Stream stream)
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
                    return Encodings.None;
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
                    return Encodings.None; // Not utf8
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
                        return Encodings.None; // Not utf8
                    }

                    --moreChars;
                }
            }

            // If we get to here then only valid UTF-8 sequences have been processed

            // If we only saw chars in the range 0-127 then we can't assume UTF8 (the caller will need to decide)
            return onlySawAsciiRange ? Encodings.Ascii : Encodings.Utf8Nobom;
        }
    }
}
