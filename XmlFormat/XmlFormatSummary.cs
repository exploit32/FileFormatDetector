using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlFormat
{
    public class XmlFormatSummary : TextFormatSummary,  IEquatable<XmlFormatSummary>
    {
        private static string[] Keys = new[] { nameof(XmlDeclarationEncoding), nameof(EncodingName), nameof(EncodingFullName), nameof(CodePage), nameof(HasBOM) };

        public override dynamic this[string key] => key switch
        {
            nameof(XmlDeclarationEncoding) => XmlDeclarationEncoding,
            nameof(EncodingName) => EncodingName,
            nameof(EncodingFullName) => EncodingFullName,
            nameof(CodePage) => CodePage,
            nameof(HasBOM) => HasBOM,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        public override string FormatName => "Xml";

        public string XmlDeclarationEncoding { get; init; } = String.Empty;

        public override bool Equals(object? obj) => this.Equals(obj as XmlFormatSummary);

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

        public override int GetHashCode() => HashCode.Combine(XmlDeclarationEncoding, EncodingName, EncodingFullName, CodePage, HasBOM);

        public override string[] GetKeys() => Keys;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(XmlDeclarationEncoding) && !XmlDeclarationEncoding.Equals(EncodingName, StringComparison.InvariantCultureIgnoreCase))
                return $"{FormatName}: {(HasBOM ? "BOM" : "no-BOM")}, {XmlDeclarationEncoding} (detected as {EncodingName})";
            else
                return $"{FormatName}: {(HasBOM ? "BOM" : "no-BOM")}, {EncodingName}";
        }
    }
}
