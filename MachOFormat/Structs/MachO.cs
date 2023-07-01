using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachOFormat.Structs
{
    internal class MachO
    {
        public MachOMagic Magic { get; init; }

        public string Architecture { get; init; } = string.Empty;

        public MachO[] InnerMachOs { get; init; }  = Array.Empty<MachO>();

        public bool IsSigned { get; init; }
    }
}
