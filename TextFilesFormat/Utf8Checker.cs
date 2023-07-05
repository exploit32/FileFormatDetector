using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class Utf8Checker
    {
        private int _moreChars = 0;

        public bool SurrogatesValid { get; private set; } = true;

        public bool FoundSurrogates { get; private set; } = false;

        public bool CheckSurrogates(ReadOnlySpan<byte> buffer)
        {
            if (SurrogatesValid)
                SurrogatesValid = CheckValidSurrogates(buffer);

            return SurrogatesValid;
        }

        private bool CheckValidSurrogates(ReadOnlySpan<byte> buffer)
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
            int size = buffer.Length;

            int pos = 0;

            while (_moreChars > 0 && pos < size)
            {
                byte ch = buffer[pos++];

                if (ch < 128 || ch > 191)
                    return false;

                FoundSurrogates = true;
                --_moreChars;
            }

            while (pos < size)
            {
                byte ch = buffer[pos++];

                if (ch == 0)
                {
                    return false;
                }

                if (ch <= 127)
                {
                    // 1 byte
                    _moreChars = 0;
                }
                else if (ch >= 194 && ch <= 223)
                {
                    // 2 Byte
                    _moreChars = 1;
                }
                else if (ch >= 224 && ch <= 239)
                {
                    // 3 Byte
                    _moreChars = 2;
                }
                else if (ch >= 240 && ch <= 244)
                {
                    // 4 Byte
                    _moreChars = 3;
                }
                else
                {
                    return false;
                }

                // Check secondary chars are in range if we are expecting any
                while (_moreChars > 0 && pos < size)
                {
                    ch = buffer[pos++];

                    if (ch < 128 || ch > 191)
                        return false;

                    FoundSurrogates = true;
                    --_moreChars;
                }
            }

            // If we get to here then only valid UTF-8 sequences have been processed
            return true;
        }
    }
}
