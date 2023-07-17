using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlFormat
{
    /// <summary>
    /// Xml files format summary
    /// </summary>
    public class XmlFormatSummary : TextFormatSummary,  IEquatable<XmlFormatSummary>
    {
        private static readonly string[] _keys = new[] { nameof(XmlDeclarationEncoding), nameof(EncodingName), nameof(EncodingFullName), nameof(CodePage), nameof(HasBOM) };

        /// <summary>
        /// Format name
        /// </summary>
        public override string FormatName => "Xml";

        /// <summary>
        /// Encoding in xml declaration or empty string
        /// </summary>
        public string XmlDeclarationEncoding { get; init; }

        /// <summary>
        /// Properties accessor
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>Property value</returns>
        /// <exception cref="KeyNotFoundException">Thrown if key is not found</exception>
        public override dynamic this[string key] => key switch
        {
            nameof(XmlDeclarationEncoding) => XmlDeclarationEncoding,
            nameof(EncodingName) => EncodingName,
            nameof(EncodingFullName) => EncodingFullName,
            nameof(CodePage) => CodePage,
            nameof(HasBOM) => HasBOM,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        internal XmlFormatSummary(string xmlDeclarationEncoding, TextFormatSummary baseTextFormat)
            : base(baseTextFormat.EncodingName, baseTextFormat.EncodingFullName, baseTextFormat.CodePage, baseTextFormat.HasBOM)
        {
            XmlDeclarationEncoding = xmlDeclarationEncoding;
        }

        public override string[] GetKeys() => _keys;

        public override bool Equals(object? obj) => this.Equals(obj as XmlFormatSummary);

        public override int GetHashCode() => HashCode.Combine(XmlDeclarationEncoding, EncodingName, EncodingFullName, CodePage, HasBOM);

        public bool Equals(XmlFormatSummary? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (XmlDeclarationEncoding == other.XmlDeclarationEncoding)
                && (EncodingName == other.EncodingName)
                && (EncodingFullName == other.EncodingFullName)
                && (CodePage == other.CodePage)
                && (HasBOM == other.HasBOM);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(XmlDeclarationEncoding) && !XmlDeclarationEncoding.Equals(EncodingName, StringComparison.InvariantCultureIgnoreCase))
                return $"{FormatName}: {(HasBOM ? "BOM" : "no-BOM")}, {XmlDeclarationEncoding} (detected as {EncodingName})";
            else
                return $"{FormatName}: {(HasBOM ? "BOM" : "no-BOM")}, {EncodingName}";
        }
    }
}
