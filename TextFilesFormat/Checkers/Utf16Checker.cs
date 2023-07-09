using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat.Checkers
{
    internal class Utf16Checker
    {
        private int _moreCharsLe = 0;

        private int _moreCharsBe = 0;

        private bool _foundLittleEndianSurrogates = false;

        private bool _foundBigEndianSurrogates = false;

        private byte[] _seenEvenBytes = new byte[256];

        private byte[] _seenOddBytes = new byte[256];

        public bool LittleEndianSurrogatesValid { get; private set; } = true;

        public bool BigEndianSurrogatesValid { get; private set; } = true;

        public bool FoundLittleEndianSurrogates => _foundLittleEndianSurrogates;

        public bool FoundBigEndianSurrogates => _foundBigEndianSurrogates;

        public bool HasNullSymbols { get; private set; } = false;


        public (int uniqueEvenBytes, int uniqueOddBytes) GetDistinctBytes()
        {
            int uniqueEvenBytes = 0;
            int uniqueOddBytes = 0;

            for (int i = 0; i < _seenEvenBytes.Length; i++)
            {
                uniqueEvenBytes += _seenEvenBytes[i];
                uniqueOddBytes += _seenOddBytes[i];
            }

            return (uniqueEvenBytes, uniqueOddBytes);
        }

        public bool CheckValidRange(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length % 2 != 0)
                throw new ArgumentException("Buffer size is expected to be a multiple of 2");

            if (LittleEndianSurrogatesValid)
                LittleEndianSurrogatesValid = CheckSurrogates(buffer, 1, ref _moreCharsLe, ref _foundLittleEndianSurrogates);

            if (BigEndianSurrogatesValid)
                BigEndianSurrogatesValid = CheckSurrogates(buffer, 0, ref _moreCharsBe, ref _foundBigEndianSurrogates);

            if (!HasNullSymbols)
                HasNullSymbols = CheckNullSymbols(buffer);

            bool utf16isPossible = (LittleEndianSurrogatesValid || BigEndianSurrogatesValid) && !HasNullSymbols;

            if (utf16isPossible)
                UpdateBytesDistribution(buffer);

            return utf16isPossible;
        }

        private void UpdateBytesDistribution(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length % 2 != 0)
                throw new ArgumentException("Buffer size is expected to be a multiple of 2");

            for (int i = 0; i < buffer.Length; i += 2)
            {
                _seenEvenBytes[buffer[i]] = 1;
            }

            for (int i = 1; i < buffer.Length; i += 2)
            {
                _seenOddBytes[buffer[i]] = 1;
            }
        }

        private bool CheckSurrogates(ReadOnlySpan<byte> buffer, int start, ref int moreChars, ref bool foundSurrogates)
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

        private bool CheckNullSymbols(ReadOnlySpan<byte> buffer)
        {
            int length = buffer.Length;
            for (int i = 0; i < length - 1; i += 2)
            {
                if (buffer[i] == 0 && buffer[i + 1] == 0)
                    return true;
            }

            return false;
        }
    }
}
