using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat.Checkers
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

            if (!HasNullSumbols && (ValidBigEndianRange || ValidLittleEndianRange))
                CheckValidBytes(buffer);

            return !HasNullSumbols && (ValidBigEndianRange || ValidLittleEndianRange);
        }

        private void CheckValidBytes(ReadOnlySpan<byte> buffer)
        {
            int length = buffer.Length;
            for (int i = 0; i < length - 3; i += 4)
            {
                byte b0 = buffer[i];
                byte b1 = buffer[i + 1];
                byte b2 = buffer[i + 2];
                byte b3 = buffer[i + 3];

                if (b0 != 0 || b1 > 10)
                    ValidBigEndianRange = false;

                if (b3 != 0 || b2 > 10)
                    ValidLittleEndianRange = false;

                if (b0 == 0 && b1 == 0 && b2 == 0 && b3 == 0)
                    HasNullSumbols = true;
            }
        }
    }
}
