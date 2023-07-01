using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    internal class Segment
    {
        /// <summary>
        /// Identifies the type of the segment.
        /// </summary>
        public SegmentType Type { get; init; }

        /// <summary>
        /// Segment-dependent flags
        /// </summary>
        public SegmentFlags Flags { get; init; }

        /// <summary>
        /// Offset of the segment in the file image.
        /// </summary>
        public ulong Offset { get; init; }

        /// <summary>
        /// Virtual address of the segment in memory. 
        /// </summary>
        public ulong VirtualAddress { get; init; }

        /// <summary>
        /// On systems where physical address is relevant, reserved for segment's physical address. 
        /// </summary>
        public ulong PhysicalAddress { get; init; }

        /// <summary>
        /// Size in bytes of the segment in the file image. May be 0. 
        /// </summary>
        public ulong FileSize { get; init; }

        /// <summary>
        /// Size in bytes of the segment in memory. May be 0. 
        /// </summary>
        public ulong MemorySize { get; init; }

        /// <summary>
        /// 0 and 1 specify no alignment. Otherwise should be a positive, integral power of 2, with p_vaddr equating p_offset modulus p_align. 
        /// </summary>
        public ulong Alignment { get; init; }        
    }
}
