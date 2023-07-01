using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEFormat.Structs
{
    internal class DosHeader
    {
        public byte[] MagicNumber { get; init; }

        public uint PEHeaderOffset { get; init; }
    }
}
