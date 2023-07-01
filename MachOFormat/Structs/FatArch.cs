namespace MachOFormat.Structs
{
    /// <summary>
    /// At the begining of the file there is one fat_header structure followed by a number of fat_arch structures.
    /// For each architecture in the file, specified by a pair of cputype and cpusubtype, the fat_header describes the file offset,
    /// file size and alignment in the file of the architecture specific member.
    /// <see cref="https://opensource.apple.com/source/cctools/cctools-895/include/mach-o/fat.h.auto.html"/>
    /// </summary>
    internal class FatArch
    {
        /// <summary>
        /// Cpu specifier
        /// </summary>
        public CpuType CpuType { get; init; }

        /// <summary>
        /// Machine specifier (int)
        /// </summary>
        public uint CpuSubtype { get; set; }

        /// <summary>
        /// File offset to this object file
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// Size of this object file
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Alignment as a power of 2
        /// </summary>
        public uint Align { get; set; }
    }
}
