using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachOFormat.Structs
{
    /// <summary>
    /// Internal representation of Mach-O file
    /// </summary>
    internal class MachO
    {
        /// <summary>
        /// Magic sequence
        /// </summary>
        public MachOMagic? Magic { get; init; }

        /// <summary>
        /// Architectire
        /// </summary>
        public string Architecture { get; init; } = string.Empty;

        /// <summary>
        /// Inner apps for FAT format
        /// </summary>
        public MachO[] InnerMachOs { get; init; }  = Array.Empty<MachO>();

        /// <summary>
        /// Is file signed
        /// </summary>
        public bool IsSigned { get; init; }
    }
}
