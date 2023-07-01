using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachOFormat.Structs
{
    /// <summary>
    /// Data from MACH-O's magic sequence
    /// </summary>
    internal class MachOMagic
    {
        /// <summary>
        /// Endianness
        /// </summary>
        public Endianness Endianness { get; set; }

        /// <summary>
        /// Is app 64 bit or 32
        /// </summary>
        public int Bits { get; set; }

        /// <summary>
        /// Is FAT format
        /// </summary>
        public bool IsFat { get; set; }
    }
}
