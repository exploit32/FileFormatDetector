using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    /// <summary>
    /// This byte is set to either 1 or 2 to signify little or big endianness, respectively. 
    /// This affects interpretation of multi-byte fields starting with offset 0x10.
    /// </summary>
    internal enum ElfEndianness: byte
    {
        LittleEndian = 1,
        BigEndian = 2,
    }
}
