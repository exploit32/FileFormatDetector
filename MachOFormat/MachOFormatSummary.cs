using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachOFormat
{
    /// <summary>
    /// Summary of Mach-O file format
    /// </summary>
    public class MachOFormatSummary : FormatSummary, IEquatable<MachOFormatSummary>
    {
        private static readonly string[] _keys = new[] { nameof(Bits), nameof(Architecture), nameof(Endianness), nameof(HasSignature), nameof(IsFat), nameof(InnerApps) };

        /// <summary>
        /// Format name
        /// </summary>
        public override string FormatName => "Mach-O";

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
        /// Indicates that file is signed
        /// </summary>
        public bool HasSignature { get; init; }

        /// <summary>
        /// Indicates that file has FAT format
        /// </summary>
        public bool IsFat { get; init; }

        /// <summary>
        /// Inner application for FAT format
        /// </summary>
        public MachOFormatSummary[] InnerApps { get; init; }

        /// <summary>
        /// Properties accessor
        /// </summary>
        /// <param name="key">Property key</param>
        /// <returns>Property value</returns>
        /// <exception cref="KeyNotFoundException">Thrown if key is not found</exception>
        public override dynamic this[string key] => key switch
        {
            nameof(Bits) => Bits,
            nameof(Architecture) => Architecture,
            nameof(Endianness) => Endianness,
            nameof(HasSignature) => HasSignature,
            nameof(IsFat) => IsFat,
            nameof(InnerApps) => InnerApps,
            _ => throw new KeyNotFoundException($"Key {key} is not supported"),
        };

        internal MachOFormatSummary(string architecture, int bits, Endianness endianness, bool hasSignature, bool isFat, MachOFormatSummary[] innerApps)
        {
            Architecture = architecture;
            Bits = bits;
            Endianness = endianness;
            HasSignature = hasSignature;
            IsFat = isFat;
            InnerApps = innerApps;            
        }
        public override string[] GetKeys() => _keys;

        public override bool Equals(object? obj) => this.Equals(obj as MachOFormatSummary);

        public override int GetHashCode()
        {
            var hashcode = HashCode.Combine(Bits, Architecture, Endianness, HasSignature, IsFat);

            if (InnerApps != null)
            {
                for (var i = 0; i < InnerApps.Length; i++)
                    hashcode ^= HashCode.Combine(i, InnerApps[i].GetHashCode());
            }

            return hashcode;
        }

        public bool Equals(MachOFormatSummary? other)
        {
            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (Bits == other.Bits)
                && (Architecture == other.Architecture)
                && (Endianness == other.Endianness)
                && (HasSignature == other.HasSignature)
                && (IsFat == other.IsFat)
                && ((InnerApps == null && other.InnerApps == null) || (InnerApps != null && InnerApps.SequenceEqual(other.InnerApps)));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder($"{FormatName}: ");

            if (!string.IsNullOrEmpty(Architecture))
                builder.Append($"{Architecture}, ");

            if (IsFat)
                builder.Append("FAT, ");

            builder.Append($"{Bits}bit-{(Endianness == Endianness.LittleEndian ? "LE" : "BE")}, ");

            if (IsFat && InnerApps != null)
                builder.Append($"{InnerApps.Length}, ");

            if (InnerApps != null && InnerApps.Length > 0)
                builder.Append(string.Join(", ", InnerApps.Select(a => $"{{{a.Bits}bit-{(a.Endianness == Endianness.LittleEndian ? "LE" : "BE")}, {a.Architecture}, {(a.HasSignature ? "signed" : "unsigned")}}}")));

            if (!IsFat)
                builder.Append($"{(HasSignature ? "signed" : "unsigned")}");

            return builder.ToString();
        }
    }
}
