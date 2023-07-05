using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextFilesFormat;

namespace TextFilesFormat
{
    internal class DetectableEncoding: IEquatable<DetectableEncoding>
    {
        public static DetectableEncoding Utf8 => new DetectableEncoding(new Signature(new byte[] { 0xEF, 0xBB, 0xBF }), Encoding.UTF8.CodePage, "utf-8", "Unicode (UTF-8)");
        public static DetectableEncoding Utf16LE => new DetectableEncoding(new Signature(new byte[] { 0xFF, 0xFE }), Encoding.Unicode.CodePage, "utf-16LE", "Unicode (UTF-16 Little-Endian)");
        public static DetectableEncoding Utf16BE => new DetectableEncoding(new Signature(new byte[] { 0xFE, 0xFF }), Encoding.BigEndianUnicode.CodePage, "utf-16BE", "Unicode (UTF-16 Big-Endian)");
        public static DetectableEncoding Utf32LE => new DetectableEncoding(new Signature(new byte[] { 0xFF, 0xFE, 0x0, 0x0 }), Encoding.UTF32.CodePage, "utf-32LE", "Unicode (UTF-32 Little-Endian)");
        public static DetectableEncoding Utf32BE => new DetectableEncoding(new Signature(new byte[] { 0x0, 0x0, 0xFE, 0xFF }), 12001, "utf-32BE", "Unicode (UTF-32 Big-Endian)");
        public static DetectableEncoding Utf7 => new DetectableEncoding(new Signature(new byte[] { 0x2B, 0x2F, 0x76 }), 65000, "utf-7", "Unicode (UTF-7)");
        public static DetectableEncoding Utf1 => new DetectableEncoding(new Signature(new byte[] { 0xF7, 0x64, 0x4C }), 0, "utf-1", "ISO-10646-UTF-1");
        public static DetectableEncoding UtfEbcdict => new DetectableEncoding(new Signature(new byte[] { 0xDD, 0x73, 0x66, 0x73 }), 0, "utf-ebcdict", "utf-ebcdict");
        public static DetectableEncoding Scsu => new DetectableEncoding(new Signature(new byte[] { 0x0E, 0xFE, 0xFF }), 0, "scsu", "Standard Compression Scheme for Unicode");
        public static DetectableEncoding Bocu1 => new DetectableEncoding(new Signature(new byte[] { 0xFB, 0xEE, 0x28 }), 0, "bocu-1", "Binary Ordered Compression for Unicode");
        public static DetectableEncoding Gb18030 => new DetectableEncoding(new Signature(new byte[] { 0x84, 0x31, 0x95, 0x33 }), 54936, "gb18030", "Chinese National Standard GB 18030-2005: Information Technology—Chinese coded character set");
        public static DetectableEncoding Windows125x => new DetectableEncoding(1251, "Windows-125x", "Windows-1251 is an 8-bit character encoding, designed to cover languages that use the Cyrillic script such as Russian, Ukrainian, Belarusian, Bulgarian, Serbian Cyrillic, Macedonian and other languages.");
        public static DetectableEncoding ASCII => new DetectableEncoding(Encoding.ASCII.CodePage, "ASCII", "Basic 7-bit encoding");

        /// <summary>
        /// Get the encoding codepage number
        /// </summary>
        /// <value>The codepage integer number</value>
        public int CodePage { get; }

        /// <summary>
        /// Get the encoding name
        /// </summary>
        /// <value>The encoding name string</value>
        public string Name { get; }

        /// <summary>
        /// Get the encoding display name
        /// </summary>
        /// <value>The encoding display name string</value>
        public string DisplayName { get; }

        /// <summary>
        /// BOM (Byte order mark) sequence
        /// </summary>
        public Signature? BomSignature { get; }

        /// <summary>
        /// Flag indicates that BOM mark exists for this encoding
        /// </summary>
        public bool HasBomSignature => BomSignature != null;

        public DetectableEncoding(Signature bomSignature, int codePage, string name, string displayName)
        {
            BomSignature = bomSignature;
            CodePage = codePage;
            Name = name;
            DisplayName = displayName;
        }

        public DetectableEncoding(int codePage, string name, string displayName) : this(null, codePage, name, displayName)
        {            
        }

        public override bool Equals(object? obj) => this.Equals(obj as DetectableEncoding);

        public bool Equals(DetectableEncoding? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (CodePage == other.CodePage)
                && (Name == other.Name)
                && (DisplayName == other.DisplayName)
                && (Equals(BomSignature, other.BomSignature));
        }

        public override string ToString()
        {
            return $"{Name}: {DisplayName}";
        }

        public override int GetHashCode() => HashCode.Combine(CodePage, Name, DisplayName, BomSignature);
    }
}
