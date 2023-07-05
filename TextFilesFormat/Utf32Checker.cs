using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class Utf32Checker
    {
        public bool ValidLittleEndianRange { get; private set; } = true;

        public bool ValidBigEndianRange { get; private set; } = true;

        public bool HasNullSumbols { get; set; } = false;

        public bool CheckValidRange(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length % 4 != 0)
                throw new ArgumentException("Buffer size is expected to be a multiple of 4");

            if (ValidBigEndianRange)
                ValidBigEndianRange = CheckValidBigEndianBytes(buffer);

            if (ValidLittleEndianRange)
                ValidLittleEndianRange = CheckValidLittleEndianBytes(buffer);

            if (!HasNullSumbols)
                HasNullSumbols = CheckNullSymbols(buffer);

            return (ValidLittleEndianRange || ValidBigEndianRange) && !HasNullSumbols;
        }

        private bool CheckValidBigEndianBytes(ReadOnlySpan<byte> buffer)
        {
            int length = buffer.Length;
            for (int i = 0; i < length - 3; i += 4)
            {
                if (buffer[i] != 0 || buffer[i + 1] > 10)
                    return false;
            }

            return true;
        }

        private bool CheckValidLittleEndianBytes(ReadOnlySpan<byte> buffer)
        {
            int length = buffer.Length;
            for (int i = 0; i < length - 3; i += 4)
            {
                if (buffer[i + 3] != 0 || buffer[i + 2] > 10)
                    return false;
            }

            return true;
        }

        private bool CheckNullSymbols(ReadOnlySpan<byte> buffer)
        {
            int length = buffer.Length;
            for (int i = 0; i < length - 3; i += 4)
            {
                if (buffer[i] == 0 && buffer[i + 1] == 0 && buffer[i + 2] == 0 && buffer[i + 3] == 0)
                    return true;
            }

            return false;
        }
    }
}
