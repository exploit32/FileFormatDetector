using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    public enum FileType : ushort
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Relocatable file
        /// </summary>
        Relocatable = 1,

        /// <summary>
        /// Executable file
        /// </summary>
        Executable = 2,

        /// <summary>
        /// Shared object
        /// </summary>
        SharedObject = 3,

        /// <summary>
        /// Core file
        /// </summary>
        Core = 4,
    }
}
