using FormatApi;

namespace PEFormat
{
    public class PEFormatSummary : FormatSummary, IEquatable<PEFormatSummary>
    {
        private static string[] Keys = new[] { nameof(Architecture), nameof(Bits), nameof(HasClrHeader), nameof(Endianness) };

        public string Architecture { get; init; }

        public int Bits { get; init; }

        public bool HasClrHeader { get; init; }

        public Endianness Endianness { get; init; }

        public override string FormatName => "PE";

        public override dynamic this[string key] => key switch
        {
            nameof(Architecture) => Architecture,
            nameof(Bits) => Bits,
            nameof(HasClrHeader) => HasClrHeader,
            nameof(Endianness) => Endianness,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        public override bool Equals(object? obj) => this.Equals(obj as PEFormatSummary);

        public override int GetHashCode() => HashCode.Combine(Architecture, Bits, HasClrHeader, Endianness);

        public override string[] GetKeys() => Keys;

        public override string ToString()
        {
            return $"{FormatName}: {Architecture}, {Bits}bit-{(Endianness == Endianness.LittleEndian ? "LE" : "BE")}, {(HasClrHeader ? "Managed" : "Unmanaged")}";
        }

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
    }
}
