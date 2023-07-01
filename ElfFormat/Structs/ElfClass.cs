using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    /// <summary>
    /// This byte is set to either 1 or 2 to signify 32- or 64-bit format, respectively.
    /// </summary>
    internal enum ElfClass: byte
    {
        Bit32 = 1,
        Bit64 = 2,
    }
}
