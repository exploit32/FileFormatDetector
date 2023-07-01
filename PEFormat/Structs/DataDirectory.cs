using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEFormat.Structs
{
    internal class DataDirectory
    {
        /// <summary>
        /// VirtualAddress, is actually the RVA of the table. The RVA is the address of the table relative to the base address of the image when the table is loaded
        /// </summary>
        public uint RVA { get; init; }

        /// <summary>
        /// Size in bytes
        /// </summary>
        public uint Size { get; init; }
    }
}
