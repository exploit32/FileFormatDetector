using FormatApi;

namespace PEFormat
{
    /// <summary>
    /// PE Format summary
    /// </summary>
    public class PEFormatSummary : FormatSummary, IEquatable<PEFormatSummary>
    {
        private static readonly string[] _keys = new[] { nameof(Architecture), nameof(Bits), nameof(HasClrHeader), nameof(Endianness) };

        /// <summary>
        /// Format name
        /// </summary>
        public override string FormatName => "PE";

        /// <summary>
        /// Architecture
        /// </summary>
        public string Architecture { get; init; }

        /// <summary>
        /// Bits
        /// </summary>
        public int Bits { get; init; }

        /// <summary>
        /// File endianness
        /// </summary>
        public Endianness Endianness { get; init; }

        /// <summary>
        /// Indicates presence of the CLR header
        /// </summary>
        public bool HasClrHeader { get; init; }

        /// <summary>
        /// Properties accessor
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>Property value</returns>
        /// <exception cref="KeyNotFoundException">Thrown if key is not found</exception>
        public override dynamic this[string key] => key switch
        {
            nameof(Architecture) => Architecture,
            nameof(Bits) => Bits,
            nameof(HasClrHeader) => HasClrHeader,
            nameof(Endianness) => Endianness,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        internal PEFormatSummary(string architecture, int bits, Endianness endianness, bool hasClrHeader)
        {
            Architecture = architecture;
            Bits = bits;
            Endianness = endianness;
            HasClrHeader = hasClrHeader;
        }

        public override string[] GetKeys() => _keys;

        public override bool Equals(object? obj) => this.Equals(obj as PEFormatSummary);

        public override int GetHashCode() => HashCode.Combine(Architecture, Bits, Endianness, HasClrHeader);
        
        public bool Equals(PEFormatSummary? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (Architecture == other.Architecture) && (Bits == other.Bits) && (HasClrHeader == other.HasClrHeader) && (Endianness == other.Endianness);
        }
        public override string ToString()
        {
            return $"{FormatName}: {Architecture}, {Bits}bit-{(Endianness == Endianness.LittleEndian ? "LE" : "BE")}, {(HasClrHeader ? "Managed" : "Unmanaged")}";
        }
    }
}
