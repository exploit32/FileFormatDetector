using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class SequentialNullsCalculator
    {
        private long _sequentialNulls = 0;

        public long MaxSequentialNulls { get; private set; } = 0;

        public bool ContainsNulls => MaxSequentialNulls > 0;

        public void ProcessBlock(ReadOnlySpan<byte> data)
        {
            int length = data.Length;

            for (int i = 0; i < length; i++)
            {
                if (data[i] == 0)
                {
                    _sequentialNulls++;

                    if (_sequentialNulls > MaxSequentialNulls)
                        MaxSequentialNulls = _sequentialNulls;
                }
                else
                {
                    _sequentialNulls = 0;
                }
            }
        }
    }
}
