namespace MachOFormat.Structs
{
    /// <summary>
    /// At the begining of the file there is one fat_header structure followed by a number of fat_arch structures.
    /// <see cref="https://opensource.apple.com/source/cctools/cctools-895/include/mach-o/fat.h.auto.html"/>
    /// </summary>
    internal class FatHeader
    {
        /// <summary>
        /// Mumber of structs that follow
        /// </summary>
        public uint NumberOfArchs { get; init; }

        /// <summary>
        /// Array of FatArch strustures that describes individual architecture
        /// </summary>
        public FatArch[] Archs { get; init; } = Array.Empty<FatArch>();
    }
}
