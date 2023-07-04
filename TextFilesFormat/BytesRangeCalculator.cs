using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class BytesRangeCalculator
    {
        public bool HasAsciiControlChars { get; private set; } = false;

        public bool HasWindows125xControlChars { get; private set; } = false;

        public bool CheckControlChars(ReadOnlySpan<byte> buffer)
        {
            if (!HasAsciiControlChars)
                HasAsciiControlChars = CheckAsciiControlChars(buffer);

            if (!HasWindows125xControlChars)
                HasWindows125xControlChars = CheckWindows125xControlChars(buffer);

            return HasAsciiControlChars || HasWindows125xControlChars;
        }

        private bool CheckAsciiControlChars(ReadOnlySpan<byte> buffer)
        {
            for(int i = 0; i < buffer.Length ; i++)
            {
                if (buffer[i] < 0x20 || buffer[i] >= 0x7F)                
                    return true;
            }

            return false;
        }

        private bool CheckWindows125xControlChars(ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] < 0x20 || buffer[i] == 0x7F)
                    return true;
            }

            return false;
        }
    }
}
