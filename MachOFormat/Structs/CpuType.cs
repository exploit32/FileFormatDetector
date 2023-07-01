namespace MachOFormat.Structs
{
    /// <summary>
    /// CPU types
    /// <see cref="https://opensource.apple.com/source/xnu/xnu-6153.11.26/osfmk/mach/machine.h.auto.html"/>
    /// </summary>
    internal enum CpuType: int
    {
        Any = -1,
        VAX = 1,
        MC680x0 = 6,
        X86 = 7,
        X86_64 = X86 | MachOConsts.CPU_ARCH_ABI64,
        MC98000 = 10,
        HPPA = 11,
        ARM = 12,
        ARM64 = ARM | MachOConsts.CPU_ARCH_ABI64,
        ARM64_32 = ARM | MachOConsts.CPU_ARCH_ABI64_32,
        MC88000 = 13,
        Sparc = 14,
        I860 = 15,
        POWERPC = 18,
        POWERPC64 = POWERPC | MachOConsts.CPU_ARCH_ABI64
    }
}
