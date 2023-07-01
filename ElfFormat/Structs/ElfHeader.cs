namespace ElfFormat.Structs
{
    internal class ElfHeader
    {
        /// <summary>
        /// 0x7F followed by ELF(45 4c 46) in ASCII; these four bytes constitute the magic number
        /// </summary>
        public byte[] Magic { get; init; }

        /// <summary>
        /// This byte is set to either 1 or 2 to signify 32- or 64-bit format, respectively.
        /// </summary>
        public ElfClass Class { get; init; }

        /// <summary>
        /// This byte is set to either 1 or 2 to signify little or big endianness, respectively. This affects interpretation of multi-byte fields starting with offset 0x10
        /// </summary>
        public ElfEndianness Endianness { get; init; }

        /// <summary>
        /// Set to 1 for the original and current version of ELF. 
        /// </summary>
        public byte Version { get; init; }

        /// <summary>
        /// Identifies the target operating system ABI.
        /// </summary>
        public OsAbi Abi { get; init; }

        /// <summary>
        /// Further specifies the ABI version. Its interpretation depends on the target ABI. Linux kernel (after at least 2.6) has no definition of it,[6] so it is ignored for statically-linked executables.
        /// </summary>
        public byte AbiVersion { get; init; }

        /// <summary>
        /// Identifies object file type.
        /// </summary>
        public FileType Type { get; init; }

        /// <summary>
        /// Specifies target instruction set architecture.
        /// </summary>
        public Machine Machine { get; init; }

        /// <summary>
        /// Set to 1 for the original version of ELF. 
        /// </summary>
        public uint ElfVersion { get; init; }

        /// <summary>
        /// This is the memory address of the entry point from where the process starts executing. 
        /// This field is either 32 or 64 bits long, depending on the format defined earlier (byte 0x04). If the file doesn't have an associated entry point, then this holds zero. 
        /// </summary>
        public ulong EntryPoint { get; init; }

        /// <summary>
        /// Points to the start of the program header table. 
        /// It usually follows the file header immediately following this one, making the offset 0x34 or 0x40 for 32- and 64-bit ELF executables, respectively. 
        /// </summary>
        public ulong ProgramHeaderOffset { get; init; }

        /// <summary>
        /// Points to the start of the section header table. 
        /// </summary>
        public ulong SectionHeaderOffset { get; init; }

        /// <summary>
        /// Interpretation of this field depends on the target architecture. 
        /// </summary>
        public uint Flags { get; init; }

        /// <summary>
        /// Contains the size of this header, normally 64 Bytes for 64-bit and 52 Bytes for 32-bit format.
        /// </summary>
        public ushort HeaderSize { get; set; }

        /// <summary>
        /// Contains the size of a program header table entry. 
        /// </summary>
        public ushort ProgramHeaderEntrySize { get; init; }

        /// <summary>
        /// Contains the number of entries in the program header table.
        /// </summary>
        public ushort ProgramHeaderEntriesCount { get; init; }

        /// <summary>
        /// Contains the size of a section header table entry. 
        /// </summary>
        public ushort SectionsHeaderEntrySize { get; init; }

        /// <summary>
        /// Contains the number of entries in the section header table. 
        /// </summary>
        public ushort SectionsHeaderEntriesCount { get; init; }

        /// <summary>
        /// Contains index of the section header table entry that contains the section names. 
        /// </summary>
        public ushort SectionsHeaderNamesIndex { get; init; }
    }
}
