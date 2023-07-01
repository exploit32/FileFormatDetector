using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PEFormat.Structs
{
    public enum DataDirectoryType
    {
        /// <summary>
        /// The export table address and size. For more information see .edata Section (Image Only)
        /// </summary>
        Export = 0,
        /// <summary>
        /// The import table address and size. For more information, see The .idata Section.
        /// </summary>
        Import = 1,
        /// <summary>
        /// The resource table address and size. For more information, see The .rsrc Section.
        /// </summary>
        Resource = 2,
        /// <summary>
        /// The exception table address and size. For more information, see The .pdata Section.
        /// </summary>
        Exception = 3,
        /// <summary>
        /// The attribute certificate table address and size. For more information, see The Attribute Certificate Table (Image Only).
        /// </summary>
        Certificate = 4,
        /// <summary>
        /// The base relocation table address and size. For more information, see The .reloc Section (Image Only).
        /// </summary>
        BaseRelocation = 5,
        /// <summary>
        /// The debug data starting address and size. For more information, see The .debug Section.
        /// </summary>
        Debug = 6,
        /// <summary>
        /// Reserved, must be 0 
        /// </summary>
        Architecture = 7,
        /// <summary>
        /// The RVA of the value to be stored in the global pointer register. The size member of this structure must be set to zero. 
        /// </summary>
        GlobalPointer = 8,
        /// <summary>
        /// The thread local storage (TLS) table address and size. For more information, see The .tls Section.
        /// </summary>
        ThreadLocalStorage = 9,
        /// <summary>
        /// The load configuration table address and size. For more information, see The Load Configuration Structure (Image Only).
        /// </summary>
        LoadConfiguration = 10,
        /// <summary>
        /// The bound import table address and size. 
        /// </summary>
        BoundImport = 11,
        /// <summary>
        /// The import address table address and size. For more information, see Import Address Table.
        /// </summary>
        ImportAddressTable = 12,
        /// <summary>
        /// The delay import descriptor address and size. For more information, see Delay-Load Import Tables (Image Only).
        /// </summary>
        DelayImport = 13,
        /// <summary>
        /// The CLR runtime header address and size. For more information, see The .cormeta Section (Object Only).
        /// </summary>
        ClrRuntime = 14,
    }
}
