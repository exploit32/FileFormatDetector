using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEFormat.Structs
{
    internal class COFFHeader
    {
        /// <summary>
        /// The Signature of the Assembly.
        /// </summary>
        public byte[]? Signature { get; init; }

        /// <summary>
        /// The target machine to run the assembly on.
        /// </summary>
        public Machine Machine { get; init; }

        /// <summary>
        /// The amount of sections in the assembly. 
        /// </summary>
        public ushort SectionCount { get; init; }

        /// <summary>
        /// The time stamp of the assembly being created
        /// </summary>
        public uint TimeDataStamp { get; init; }

        /// <summary>
        /// A pointer to the Symbol table.
        /// </summary>
        public uint PointerToSymbolTable { get; init; }

        /// <summary>
        /// The number of symbols in the symbol table
        /// </summary>
        public uint NumberOfSymbols { get; init; }

        /// <summary>
        /// The Size of the optional header in bytes.
        /// </summary>
        public ushort SizeOfOptionalHeader { get; init; }

        /// <summary>
        /// The characteristics of the Header.
        /// </summary>
        public Characteristics Characteristics { get; init; }

        public int GetBitsByMachineType()
        {
            int bits = 0;

            switch (Machine)
            {
                case Machine.ALPHA:
                case Machine.AM33:
                case Machine.ARM:
                case Machine.ARMNT:
                case Machine.I386:
                case Machine.LOONGARCH32:
                case Machine.M32R:
                case Machine.MIPS16:
                case Machine.MIPSFPU:
                case Machine.MIPSFPU16:
                case Machine.POWERPC:
                case Machine.POWERPCFP:
                case Machine.RISCV32:
                case Machine.SH3:
                case Machine.SH3DSP:
                case Machine.SH4:
                case Machine.THUMB:
                case Machine.WCEMIPSV2:
                    bits = 32;
                    break;
                case Machine.ALPHA64:
                case Machine.AMD64:
                case Machine.ARM64:
                case Machine.EBC:
                case Machine.LOONGARCH64:
                case Machine.R4000:
                case Machine.RISCV64:
                case Machine.SH5:
                    bits = 64;
                    break;
                case Machine.RISCV128:
                    bits = 128;
                    break;
            }

            return bits;
        }
    }
}
