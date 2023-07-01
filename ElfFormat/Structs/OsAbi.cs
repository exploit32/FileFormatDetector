using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat.Structs
{
    internal enum OsAbi: byte
    {
        /// <summary>
        /// System V
        /// </summary>
        SystemV = 0x00,

        /// <summary>
        /// HP-UX
        /// </summary>
        HPUX = 0x01,

        /// <summary>
        /// NetBSD
        /// </summary>
        NetBSD = 0x02,

        /// <summary>
        /// Linux
        /// </summary>
        Linux = 0x03,

        /// <summary>
        /// GNU Hurd
        /// </summary>
        GNUHurd = 0x04,

        /// <summary>
        /// Solaris
        /// </summary>
        Solaris = 0x06,

        /// <summary>
        /// AIX (Monterey)
        /// </summary>
        AIX = 0x07,

        /// <summary>
        /// IRIX
        /// </summary>
        IRIX = 0x08,

        /// <summary>
        /// FreeBSD
        /// </summary>
        FreeBSD = 0x09,

        /// <summary>
        /// Tru64
        /// </summary>
        Tru64 = 0x0A,

        /// <summary>
        /// Novell Modesto
        /// </summary>
        NovellModesto = 0x0B,

        /// <summary>
        /// OpenBSD
        /// </summary>
        OpenBSD = 0x0C,

        /// <summary>
        /// OpenVMS
        /// </summary>
        OpenVMS = 0x0D,

        /// <summary>
        /// NonStop Kernel
        /// </summary>
        NonStopKernel = 0x0E,

        /// <summary>
        /// AROS
        /// </summary>
        AROS = 0x0F,

        /// <summary>
        /// FenixOS
        /// </summary>
        FenixOS = 0x10,

        /// <summary>
        /// Nuxi CloudABI
        /// </summary>
        NuxiCloudABI = 0x11,

        /// <summary>
        /// Stratus Technologies OpenVOS
        /// </summary>
        StratusTechnologiesOpenVOS = 0x12,
    }
}
