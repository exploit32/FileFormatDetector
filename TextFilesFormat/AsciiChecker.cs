using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class AsciiChecker
    {
        public bool HasValuesGreater0x7F { get; private set; } = false;

        public bool HasControlCharsLower0x20 { get; set; } = false;

        public bool HasNulls { get; private set; } = false;

        public bool MayBeAscii => !HasNulls && !HasValuesGreater0x7F && !HasControlCharsLower0x20;

        public bool MayBeWindows1251 => !HasNulls && !HasControlCharsLower0x20;

        public bool CheckValidRange(ReadOnlySpan<byte> buffer)
        {
            if (!HasControlCharsLower0x20)
                HasControlCharsLower0x20 = CheckLowerControlChars(buffer);

            if (!HasValuesGreater0x7F)
                HasValuesGreater0x7F = CheckValuesGreater0x7F(buffer);

            if (!HasNulls)
                HasNulls = CheckNulls(buffer);

            return !HasValuesGreater0x7F || !HasValuesGreater0x7F || !HasNulls;
        }

        private bool CheckLowerControlChars(ReadOnlySpan<byte> buffer)
        {
            for(int i = 0; i < buffer.Length ; i++)
            {
                if (buffer[i] < 0x20 && buffer[i] != '\r' && buffer[i] != '\n' && buffer[i] != '\t')
                    return true;
            }

            return false;
        }

        private bool CheckValuesGreater0x7F(ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] >= 0x7F)
                    return true;
            }

            return false;
        }

        private bool CheckNulls(ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i] == 0)
                    return true;

            return false;
        }
    }
}
