using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextFilesFormat
{
    internal enum Encodings
    {
        None, // Unknown or binary
        Ansi, // 0-255
        Ascii, // 0-127
        Utf8Bom, // UTF8 with BOM
        Utf8Nobom, // UTF8 without BOM
        Utf16LeBom, // UTF16 LE with BOM
        Utf16LeNoBom, // UTF16 LE without BOM
        Utf16BeBom, // UTF16-BE with BOM
        Utf16BeNoBom // UTF16-BE without BOM
    }
}
