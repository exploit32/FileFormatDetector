namespace MachOFormat.Structs
{
    /// <summary>
    /// App header structure
    /// </summary>
    internal class AppHeader
    {
        /// <summary>
        /// CPU type
        /// </summary>
        public CpuType CpuType { get; init; }

        /// <summary>
        /// CPU subtype (individual for each CPU family)
        /// </summary>
        public uint CpuSubtype { get; init; }

        /// <summary>
        /// File type
        /// </summary>
        public uint FileType { get; init; }

        /// <summary>
        /// Number of load commands
        /// </summary>
        public uint NumberOfLoadCommands { get; init; }

        /// <summary>
        /// Total size of load commands
        /// </summary>
        public uint SizeOfLoadCommands { get; init; }

        /// <summary>
        /// Flags
        /// </summary>
        public uint Flags { get; init; }
    }
}
