namespace MachOFormat.Structs
{
    internal class AppHeader
    {
        /// <summary>
        /// CPU type
        /// </summary>
        public CpuType CpuType { get; init; }

        public uint CpuSubtype { get; init; }

        public uint FileType { get; init; }

        public uint NumberOfLoadCommands { get; init; }

        public uint SizeOfLoadCommands { get; init; }

        public uint Flags { get; init; }
    }
}
