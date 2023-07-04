using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class Utf32Checker
    {
        public long LittleEndianSymbols { get; private set; }

        public long BigEndianSymbols { get; private set; }

        public bool HasNullTriples => LittleEndianSymbols + BigEndianSymbols > 0;

        public void ProcessBlock(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length % 4 != 0)
                throw new ArgumentException("Buffer size is expected to ba a multiple of 4");

            int length = buffer.Length;
            for (int i = 0; i < length; i += 4)
            {
                if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0 && buffer[3] != 0)
                    BigEndianSymbols++;

                if (buffer[0] != 0 && buffer[1] == 0 && buffer[2] == 0 && buffer[3] == 0)
                    LittleEndianSymbols++;
            }
        }
    }
}
