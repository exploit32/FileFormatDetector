using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat
{
    public class ElfFormatSummary : FormatSummary, IEquatable<ElfFormatSummary>
    {
        private static string[] Keys = new[] { nameof(Architecture), nameof(Bits), nameof(Interpreter), nameof(Endianness) };

        public override dynamic this[string key] => key switch
        {
            nameof(Architecture) => Architecture,
            nameof(Bits) => Bits,
            nameof(Endianness) => Endianness,
            nameof(Interpreter) => Interpreter,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        public string Architecture { get; init; }

        public int Bits { get; init; }

        public Endianness Endianness { get; init; }

        public string Interpreter { get; init; }

        public override string FormatName => "ELF";

        public override bool Equals(object? obj) => this.Equals(obj as ElfFormatSummary);

        public bool Equals(ElfFormatSummary? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (Architecture == other.Architecture) && (Bits == other.Bits) && (Interpreter == other.Interpreter) && (Endianness == other.Endianness);
        }

        public override int GetHashCode() => HashCode.Combine(Architecture, Bits, Interpreter, Endianness);

        public override string[] GetKeys() => Keys;

        public override string ToString()
        {
            return $"{FormatName}: {Architecture}, {Bits}bit-{(Endianness == Endianness.LittleEndian ? "LE" : "BE")}{(!string.IsNullOrEmpty(Interpreter) ? ", " + Interpreter : "")}";
        }
    }
}
