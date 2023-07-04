using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class BytesRangeCalculator
    {
        public bool AllBytesLessThan128(ReadOnlySpan<byte> buffer)
        {
            for(int i = 0; i < buffer.Length ; i++)
            {
                if (buffer[i] >= 128)                
                    return false;
            }

            return true;
        }
    }
}
