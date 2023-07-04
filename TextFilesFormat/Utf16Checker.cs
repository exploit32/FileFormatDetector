using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class Utf16Checker
    {
        private int _moreCharsLe = 0;

        private int _moreCharsBe = 0;

        private bool _foundLittleEndianSurrogates = false;

        private bool _foundBigEndianSurrogates = false;

        public bool LittleEndianSurrogatesValid { get; private set; } = true;


        public bool BigEndianSurrogatesValid { get; private set; } = true;

        public bool FoundLittleEndianSurrogates => _foundLittleEndianSurrogates;

        public bool FoundBigEndianSurrogates => _foundBigEndianSurrogates;

        public bool CheckValidSurrogates(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length % 2 != 0)
                throw new ArgumentException("Buffer size is expected to be a multiple of 2");

            if (LittleEndianSurrogatesValid)
                LittleEndianSurrogatesValid = CheckSurrogates(buffer, 1, ref _moreCharsLe, ref _foundLittleEndianSurrogates);

            if (BigEndianSurrogatesValid)
                BigEndianSurrogatesValid = CheckSurrogates(buffer, 0, ref _moreCharsBe, ref _foundBigEndianSurrogates);

            return LittleEndianSurrogatesValid | BigEndianSurrogatesValid;
        }

        public bool CheckSurrogates(ReadOnlySpan<byte> buffer, int start, ref int moreChars, ref bool foundSurrogates)
        {
            int size = buffer.Length;

            int pos = start;

            while (moreChars > 0 && pos < size)
            {
                byte ch = buffer[pos];
                pos += 2;

                if (ch < 0xDC || ch > 0xDF)
                    return false;

                foundSurrogates = true;
                --moreChars;
            }

            while (pos < size)
            {
                byte currentByte = buffer[pos];
                pos += 2;

                if (currentByte >= 0xD8 && currentByte < 0xE0)
                {
                    moreChars = 1;
                }

                while (moreChars > 0 && pos < size)
                {
                    byte nextByte = buffer[pos];
                    pos += 2;

                    if (nextByte < 0xDC || nextByte > 0xDF)
                        return false;

                    foundSurrogates = true;
                    --moreChars;
                }
            }

            return true;
        }
    }
}
