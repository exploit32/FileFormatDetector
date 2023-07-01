using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    internal enum SegmentType: uint
    {
        /// <summary>
        /// Program header table entry unused
        /// </summary>
        Null = 0,

        /// <summary>
        /// Loadable segment
        /// </summary>
        Loadable = 1,

        /// <summary>
        /// Dynamic linking information
        /// </summary>
        Dynamic = 2,

        /// <summary>
        /// Interpreter information
        /// </summary>
        Interpreter = 3,

        /// <summary>
        /// Auxiliary information
        /// </summary>
        Note = 4,

        /// <summary>
        /// Reserved
        /// </summary>
        SharedLibrary = 5,

        /// <summary>
        /// Segment containing program header table itself
        /// </summary>
        ProgramHeader = 6,

        /// <summary>
        /// Thread-Local Storage template
        /// </summary>
        ThreadLocalStorageTemplate = 7,
    }
}
