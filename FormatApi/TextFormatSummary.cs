using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FormatApi
{
    /// <summary>
    /// Base class for text and text-based formats
    /// </summary>
    public class TextFormatSummary : FormatSummary, IEquatable<TextFormatSummary>
    {
        private static readonly string[] _keys = new[] { nameof(EncodingName), nameof(EncodingFullName), nameof(CodePage), nameof(HasBOM) };

        /// <summary>
        /// Format name
        /// </summary>
        public override string FormatName => "Text";

        /// <summary>
        /// Encoding short name
        /// </summary>
        public string EncodingName { get; init; }

        /// <summary>
        /// Encoding full name
        /// </summary>
        public string EncodingFullName { get; init; }

        /// <summary>
        /// Encoding code page
        /// </summary>
        public int CodePage { get; init; }

        /// <summary>
        /// Indicates that file has byte-order-mark sequence
        /// </summary>
        public bool HasBOM { get; init; }

        /// <summary>
        /// Properties accessor
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>Property value</returns>
        /// <exception cref="KeyNotFoundException">Thrown if key is not found</exception>
        public override dynamic this[string key] => key switch
        {
            nameof(EncodingName) => EncodingName,
            nameof(EncodingFullName) => EncodingFullName,
            nameof(CodePage) => CodePage,
            nameof(HasBOM) => HasBOM,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        public TextFormatSummary(string encodingName, string encodingFullName, int codePage, bool hasBOM)
        {
            EncodingName = encodingName;
            EncodingFullName = encodingFullName;
            CodePage = codePage;
            HasBOM = hasBOM;
        }
        public override string[] GetKeys() => _keys;

        public override bool Equals(object? obj) => this.Equals(obj as TextFormatSummary);

        public override int GetHashCode() => HashCode.Combine(EncodingName, EncodingFullName, CodePage, HasBOM);

        public bool Equals(TextFormatSummary? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (EncodingName == other.EncodingName)
                && (EncodingFullName == other.EncodingFullName)
                && (CodePage == other.CodePage)
                && (HasBOM == other.HasBOM);
        }

        public override string ToString()
        {
            return $"{FormatName}: {(HasBOM ? "BOM" : "no-BOM")}, {EncodingName}";
        }
    }
}
