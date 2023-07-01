using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MachOFormat.Structs
{
    /// <summary>
    /// Load command types
    /// <see cref="https://opensource.apple.com/source/xnu/xnu-4570.71.2/EXTERNAL_HEADERS/mach-o/loader.h.auto.html"/>
    /// </summary>
    internal enum LoadCommandType : uint
    {
        /// <summary>
        /// segment of this file to be mapped
        /// </summary>
        LC_SEGMENT = 0x1,

        /// <summary>
        /// link-edit stab symbol table info
        /// </summary>
        LC_SYMTAB = 0x2,

        /// <summary>
        /// link-edit gdb symbol table info (obsolete)
        /// </summary>
        LC_SYMSEG = 0x3,

        /// <summary>
        /// thread
        /// </summary>
        LC_THREAD = 0x4,

        /// <summary>
        /// unix thread (includes a stack)
        /// </summary>
        LC_UNIXTHREAD = 0x5,

        /// <summary>
        /// load a specified fixed VM shared library
        /// </summary>
        LC_LOADFVMLIB = 0x6,

        /// <summary>
        /// fixed VM shared library identification
        /// </summary>
        LC_IDFVMLIB = 0x7,

        /// <summary>
        /// object identification info (obsolete)
        /// </summary>
        LC_IDENT = 0x8,

        /// <summary>
        /// fixed VM file inclusion (internal use)
        /// </summary>
        LC_FVMFILE = 0x9,

        /// <summary>
        /// prepage command(internal use)
        /// </summary>
        LC_PREPAGE = 0xa,

        /// <summary>
        /// dynamic link-edit symbol table info
        /// </summary>
        LC_DYSYMTAB = 0xb,

        /// <summary>
        /// load a dynamically linked shared library
        /// </summary>
        LC_LOAD_DYLIB = 0xc,

        /// <summary>
        /// dynamically linked shared lib ident
        /// </summary>
        LC_ID_DYLIB = 0xd,

        /// <summary>
        /// load a dynamic linker
        /// </summary>
        LC_LOAD_DYLINKER = 0xe,

        /// <summary>
        /// dynamic linker identification
        /// </summary>
        LC_ID_DYLINKER = 0xf,

        /// <summary>
        /// modules prebound for a dynamically linked shared library
        /// </summary>
        LC_PREBOUND_DYLIB = 0x10,

        /// <summary>
        /// image routines
        /// </summary>
        LC_ROUTINES = 0x11,

        /// <summary>
        /// sub framework
        /// </summary>
        LC_SUB_FRAMEWORK = 0x12,

        /// <summary>
        /// sub umbrella
        /// </summary>
        LC_SUB_UMBRELLA = 0x13,

        /// <summary>
        /// sub client
        /// </summary>
        LC_SUB_CLIENT = 0x14,

        /// <summary>
        /// sub library
        /// </summary>
        LC_SUB_LIBRARY = 0x15,

        /// <summary>
        /// two-level namespace lookup hints
        /// </summary>
        LC_TWOLEVEL_HINTS = 0x16,

        /// <summary>
        /// prebind checksum
        /// </summary>
        LC_PREBIND_CKSUM = 0x17,

        /// <summary>
        /// load a dynamically linked shared library that is allowed to be missing (all symbols are weak imported).
        /// </summary>
        LC_LOAD_WEAK_DYLIB = (0x18 | MachOConsts.LC_REQ_DYLD),

        /// <summary>
        /// 64-bit segment of this file to be = mapped
        /// </summary>
        LC_SEGMENT_64 = 0x19,

        /// <summary>
        /// 64-bit image routines
        /// </summary>
        LC_ROUTINES_64 = 0x1a,

        /// <summary>
        /// the uuid
        /// </summary>
        LC_UUID = 0x1b,

        /// <summary>
        /// runpath additions
        /// </summary>
        LC_RPATH = (0x1c | MachOConsts.LC_REQ_DYLD),

        /// <summary>
        /// local of code signature
        /// </summary>
        LC_CODE_SIGNATURE = 0x1d,

        /// <summary>
        /// local of info to split segments
        /// </summary>
        LC_SEGMENT_SPLIT_INFO = 0x1e,

        /// <summary>
        /// load and re-export dylib
        /// </summary>
        LC_REEXPORT_DYLIB = (0x1f | MachOConsts.LC_REQ_DYLD),

        /// <summary>
        /// delay load of dylib until first use
        /// </summary>
        LC_LAZY_LOAD_DYLIB = 0x20,

        /// <summary>
        /// encrypted segment information
        /// </summary>
        LC_ENCRYPTION_INFO = 0x21,

        /// <summary>
        /// compressed dyld information
        /// </summary>
        LC_DYLD_INFO = 0x22,

        /// <summary>
        /// compressed dyld information only
        /// </summary>
        LC_DYLD_INFO_ONLY = (0x22 | MachOConsts.LC_REQ_DYLD),

        /// <summary>
        /// load upward dylib
        /// </summary>
        LC_LOAD_UPWARD_DYLIB = (0x23 | MachOConsts.LC_REQ_DYLD),

        /// <summary>
        /// build for MacOSX min OS version
        /// </summary>
        LC_VERSION_MIN_MACOSX = 0x24,

        /// <summary>
        /// build for iPhoneOS min OS version
        /// </summary>
        LC_VERSION_MIN_IPHONEOS = 0x25,

        /// <summary>
        /// compressed table of function start addresses
        /// </summary>
        LC_FUNCTION_STARTS = 0x26,

        /// <summary>
        /// string for dyld to treat like environment variable
        /// </summary>
        LC_DYLD_ENVIRONMENT = 0x27,

        /// <summary>
        /// replacement for LC_UNIXTHREAD
        /// </summary>
        LC_MAIN = (0x28 | MachOConsts.LC_REQ_DYLD),

        /// <summary>
        /// table of non-instructions in __text
        /// </summary>
        LC_DATA_IN_CODE = 0x29,

        /// <summary>
        /// source version used to build binary
        /// </summary>
        LC_SOURCE_VERSION = 0x2A,

        /// <summary>
        /// Code signing DRs copied from linked dylibs
        /// </summary>
        LC_DYLIB_CODE_SIGN_DRS = 0x2B,
    }
}
