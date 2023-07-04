using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class EvenOddNullsCalculator
    {
        public long EvenNulls { get; private set; }

        public long OddNulls { get; private set; }

        public void ProcessBlock(ReadOnlySpan<byte> data)
        {
           int length = data.Length;
            
            for (int i = 0; i < length - 1; i += 2)
            {
                if (data[i] == 0)
                    EvenNulls++;

                if (data[i + 1] == 0)
                    OddNulls++;
            }

            if (length % 2 != 0 && data[length - 1] == 0)
                EvenNulls++;
        }
    }
}
