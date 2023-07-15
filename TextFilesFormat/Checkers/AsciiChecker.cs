using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat.Checkers
{
    internal class AsciiChecker
    {
        public bool HasValuesGreater0x7F { get; private set; } = false;

        public bool HasControlCharsLower0x20 { get; set; } = false;

        public bool MayBeAscii => !HasValuesGreater0x7F && !HasControlCharsLower0x20;

        public bool MayBeWindows1251 => !HasControlCharsLower0x20;

        public bool CheckValidRange(ReadOnlySpan<byte> buffer)
        {
            if (!HasControlCharsLower0x20 || !HasValuesGreater0x7F)
                CheckLowerControlChars(buffer);

            return !HasValuesGreater0x7F || !HasValuesGreater0x7F;
        }

        private void CheckLowerControlChars(ReadOnlySpan<byte> buffer)
        {
            foreach (byte c in buffer)
            {
                if (c < 0x20 && c != '\r' && c != '\n' && c != '\t')
                    HasControlCharsLower0x20 = true;

                if (c >= 0x7F)
                    HasValuesGreater0x7F = true;
            }
        }
    }
}
