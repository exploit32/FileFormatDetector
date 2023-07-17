using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat.Checkers
{
    /// <summary>
    /// Class that looks for control characters and greater than 0x7F
    /// </summary>
    internal class AsciiChecker
    {
        /// <summary>
        /// Indicates that bytes greater that 7F were found
        /// </summary>
        public bool HasValuesGreater0x7F { get; private set; } = false;

        /// <summary>
        /// Indicates that control chars were found
        /// </summary>
        public bool HasControlCharsLower0x20 { get; set; } = false;

        /// <summary>
        /// Indicates that all bytes are in valid ASCII range
        /// </summary>
        public bool MayBeAscii => !HasValuesGreater0x7F && !HasControlCharsLower0x20;

        /// <summary>
        /// Indicates that file might be one-byte Windows-125x encoding
        /// </summary>
        public bool MayBeWindows125x => !HasControlCharsLower0x20;

        /// <summary>
        /// Check provided block of bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
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
