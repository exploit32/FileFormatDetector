using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfFormat
{
    /// <summary>
    /// Summary of ELF file format
    /// </summary>
    public class ElfFormatSummary : FormatSummary, IEquatable<ElfFormatSummary>
    {
        private static readonly string[] _keys = new[] { nameof(Architecture), nameof(Bits), nameof(Interpreter), nameof(Endianness) };

        /// <summary>
        /// Format name
        /// </summary>
        public override string FormatName => "ELF";

        /// <summary>
        /// Target architectore
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
        /// Interpreter
        /// </summary>
        public string Interpreter { get; init; }

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
            nameof(Endianness) => Endianness,
            nameof(Interpreter) => Interpreter,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        internal ElfFormatSummary(string architecture, int bits, Endianness endianness, string interpreter)
        {
            Architecture = architecture;
            Bits = bits;
            Endianness = endianness;
            Interpreter = interpreter;
        }
        public override string[] GetKeys() => _keys;

        public override bool Equals(object? obj) => this.Equals(obj as ElfFormatSummary);

        public override int GetHashCode() => HashCode.Combine(Architecture, Bits, Interpreter, Endianness);

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

        public override string ToString()
        {
            return $"{FormatName}: {Architecture}, {Bits}bit-{(Endianness == Endianness.LittleEndian ? "LE" : "BE")}{(!string.IsNullOrEmpty(Interpreter) ? ", " + Interpreter : "")}";
        }
    }
}
