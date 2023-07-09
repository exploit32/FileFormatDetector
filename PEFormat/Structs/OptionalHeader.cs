using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace PEFormat.Structs
{
    internal class OptionalHeader
    {
        public enum PEFormat : ushort
        {
            PE32 = 0x10b,
            ROM = 0x107,
            PE32Plus = 0x20b,
        }

        /// <summary>
        /// The unsigned integer that identifies the state of the image file.
        /// The most common number is 0x10B, which identifies it as a normal executable file.
        /// 0x107 identifies it as a ROM image, and 0x20B identifies it as a PE32+ executable.
        /// </summary>
        public PEFormat Magic { get; init; }

        /// <summary>
        /// The linker major version number. 
        /// </summary>
        public byte MajorLinkerVersion { get; init; }

        /// <summary>
        /// The linker minor version number. 
        /// </summary>
        public byte MinorLinkerVersion { get; init; }

        /// <summary>
        /// The size of the code (text) section, or the sum of all code sections if there are multiple sections. 
        /// </summary>
        public uint SizeOfCode { get; init; }

        /// <summary>
        /// The size of the initialized data section, or the sum of all such sections if there are multiple data sections. 
        /// </summary>
        public uint SizeOfInitializedData { get; init; }

        /// <summary>
        /// The size of the uninitialized data section (BSS), or the sum of all such sections if there are multiple BSS sections. 
        /// </summary>
        public uint SizeOfUninitializedData { get; init; }

        /// <summary>
        /// The address of the entry point relative to the image base when the executable file is loaded into memory. 
        /// For program images, this is the starting address. 
        /// For device drivers, this is the address of the initialization function. 
        /// An entry point is optional for DLLs. When no entry point is present, this field must be zero. 
        /// </summary>
        public uint AddressOfEntryPoint { get; init; }

        /// <summary>
        /// The address that is relative to the image base of the beginning-of-code section when it is loaded into memory. 
        /// </summary>
        public uint BaseOfCode { get; init; }

        /// <summary>
        /// PE32 only: The address that is relative to the image base of the beginning-of-data section when it is loaded into memory. 
        /// </summary>
        public uint BaseOfData { get; init; }

        /// <summary>
        /// The preferred address of the first byte of image when loaded into memory; must be a multiple of 64 K. 
        /// The default for DLLs is 0x10000000. 
        /// The default for Windows CE EXEs is 0x00010000. 
        /// The default for Windows NT, Windows 2000, Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000. 
        /// </summary>
        public ulong ImageBase { get; init; }

        /// <summary>
        /// The alignment (in bytes) of sections when they are loaded into memory. It must be greater than or equal to FileAlignment. The default is the page size for the architecture. 
        /// </summary>
        public uint SectionAlignment { get; init; }

        /// <summary>
        /// The alignment factor (in bytes) that is used to align the raw data of sections in the image file. 
        /// The value should be a power of 2 between 512 and 64 K, inclusive. 
        /// The default is 512. If the SectionAlignment is less than the architecture's page size, then FileAlignment must match SectionAlignment. 
        /// </summary>
        public uint FileAlignment { get; init; }

        /// <summary>
        /// The major version number of the required operating system.
        /// </summary>
        public ushort MajorOperatingSystemVersion { get; init; }

        /// <summary>
        /// The minor version number of the required operating system. 
        /// </summary>
        public ushort MinorOperatingSystemVersion { get; init; }

        /// <summary>
        /// The major version number of the image. 
        /// </summary>
        public ushort MajorImageVersion { get; init; }

        /// <summary>
        /// The minor version number of the image. 
        /// </summary>
        public ushort MinorImageVersion { get; init; }

        /// <summary>
        /// The major version number of the subsystem. 
        /// </summary>
        public ushort MajorSubsystemVersion { get; init; }

        /// <summary>
        /// The minor version number of the subsystem. 
        /// </summary>
        public ushort MinorSubsystemVersion { get; init; }

        /// <summary>
        /// Reserved, must be zero. 
        /// </summary>
        public uint Win32VersionValue { get; init; }

        /// <summary>
        /// The size (in bytes) of the image, including all headers, as the image is loaded in memory. It must be a multiple of SectionAlignment. 
        /// </summary>
        public uint SizeOfImage { get; init; }

        /// <summary>
        /// The combined size of an MS-DOS stub, PE header, and section headers rounded up to a multiple of FileAlignment. 
        /// </summary>
        public uint SizeOfHeaders { get; init; }

        /// <summary>
        /// The image file checksum. The algorithm for computing the checksum is incorporated into IMAGHELP.DLL. 
        /// The following are checked for validation at load time: all drivers, any DLL loaded at boot time, and any DLL that is loaded into a critical Windows process. 
        /// </summary>
        public uint CheckSum { get; init; }

        /// <summary>
        /// The subsystem that is required to run this image. For more information, see Windows Subsystem. 
        /// </summary>
        public ushort Subsystem { get; init; }

        /// <summary>
        /// For more information, see DLL Characteristics later in this specification.
        /// </summary>
        public ushort DllCharacteristics { get; init; }

        /// <summary>
        /// The size of the stack to reserve. Only SizeOfStackCommit is committed; the rest is made available one page at a time until the reserve size is reached. 
        /// </summary>
        public ulong SizeOfStackReserve { get; init; }

        /// <summary>
        /// The size of the stack to commit. 
        /// </summary>
        public ulong SizeOfStackCommit { get; init; }

        /// <summary>
        /// The size of the local heap space to reserve. Only SizeOfHeapCommit is committed; the rest is made available one page at a time until the reserve size is reached. 
        /// </summary>
        public ulong SizeOfHeapReserve { get; init; }

        /// <summary>
        /// The size of the local heap space to commit. 
        /// </summary>
        public ulong SizeOfHeapCommit { get; init; }

        /// <summary>
        /// Reserved, must be zero. 
        /// </summary>
        public uint LoaderFlags { get; init; }

        /// <summary>
        /// The number of data-directory entries in the remainder of the optional header. Each describes a location and size. 
        /// </summary>
        public uint NumberOfRvaAndSizes { get; init; }

        public DataDirectory[] DataDirectories { get; set; } = Array.Empty<DataDirectory>();
    }
}
