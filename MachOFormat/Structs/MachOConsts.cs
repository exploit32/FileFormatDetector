namespace MachOFormat.Structs
{
    internal static class MachOConsts
    {
        /// <summary>
        /// After MacOS X 10.1 when a new load command is added that is required to be understood by the dynamic linker
        /// for the image to execute properly the LC_REQ_DYLD bit will be or'ed into the load command constant.
        /// If the dynamic linker sees such a load command it it does not understand will 
        /// issue a "unknown load command required for execution" error and refuse to use the image. 
        /// Other load commands without this bit that are not understood will simply be ignored.
        /// </summary>
        internal const uint LC_REQ_DYLD = 0x80000000;

        /// <summary>
        /// 64 bit ABI
        /// <see cref="https://opensource.apple.com/source/xnu/xnu-6153.11.26/osfmk/mach/machine.h.auto.html"/>
        /// </summary>
        public const int CPU_ARCH_ABI64 = 0x01000000;

        /// <summary>
        /// ABI for 64-bit hardware with 32-bit types; LP32
        /// <see cref="https://opensource.apple.com/source/xnu/xnu-6153.11.26/osfmk/mach/machine.h.auto.html"/>
        /// </summary>
        public const int CPU_ARCH_ABI64_32 = 0x02000000;

        public const int LoadCommandHeaderSize = 8;
    }
}
