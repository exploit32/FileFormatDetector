using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal class FileSummary
    {
        public bool ContainsNulls => NullsAt0Position + NullsAt1Position + NullsAt2Position + NullsAt3Position > 0;

        public bool ContainsNullTriples => LittleEndianNullTriples + BigEndianNullTriples > 0;

        public bool OnlyAsciiRange { get; set; }

        public bool ValidUtf8Sequences { get; set; }

        public long NullsAt0Position { get; set; }

        public long NullsAt1Position { get; set; }

        public long NullsAt2Position { get; set; }

        public long NullsAt3Position { get; set; }

        public long LittleEndianNullTriples { get; set; }

        public long BigEndianNullTriples { get; set; }

        public long SequentialNulls { get; set; }
    }
}
