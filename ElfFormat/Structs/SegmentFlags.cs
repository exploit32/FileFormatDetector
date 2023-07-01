using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    [Flags]
    internal enum SegmentFlags: uint
    {
        /// <summary>
        /// Invalid value
        /// </summary>
        None = 0,

        /// <summary>
        /// Executable segment
        /// </summary>
        Executable = 1,

        /// <summary>
        /// Writeable segment
        /// </summary>
        Writeable = 2,

        /// <summary>
        /// Readable segment
        /// </summary>
        Readable = 4,
    }
}
