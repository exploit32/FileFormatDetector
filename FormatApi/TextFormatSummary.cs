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
        private static string[] Keys = new[] { nameof(EncodingName), nameof(EncodingFullName), nameof(CodePage), nameof(HasBOM) };

        public override dynamic this[string key] => key switch
        {
            nameof(EncodingName) => EncodingName,
            nameof(EncodingFullName) => EncodingFullName,
            nameof(CodePage) => CodePage,
            nameof(HasBOM) => HasBOM,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        public override string FormatName => "Text";

        public string EncodingName { get; init; } = string.Empty;

        public string EncodingFullName { get; init; } = string.Empty;

        public int CodePage { get; init; }

        public bool HasBOM { get; init; }

        public override bool Equals(object? obj) => this.Equals(obj as TextFormatSummary);

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

        public override int GetHashCode() => HashCode.Combine(EncodingName, EncodingFullName, CodePage, HasBOM);

        public override string[] GetKeys() => Keys;

        public override string ToString()
        {
            return $"{FormatName}: {(HasBOM ? "BOM" : "no-BOM")}, {EncodingName}";
        }
    }
}
