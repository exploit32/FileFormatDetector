using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MachOFormat
{
    public class MachOFormatSummary : FormatSummary, IEquatable<MachOFormatSummary>
    {
        private static string[] Keys = new[] { nameof(Bits), nameof(Architecture), nameof(Endianness), nameof(HasSignature), nameof(IsFat), nameof(InnerApps) };

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

        public override string FormatName => "Mach-O";

        public int Bits { get; init; }

        public string Architecture { get; init; } = String.Empty;

        public Endianness Endianness { get; init; }

        public bool HasSignature { get; init; }

        public bool IsFat { get; init; }

        public MachOFormatSummary[] InnerApps { get; init; } = Array.Empty<MachOFormatSummary>();

        public override bool Equals(object? obj) => this.Equals(obj as MachOFormatSummary);

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

        public override int GetHashCode() => HashCode.Combine(Bits, Architecture, Endianness, HasSignature, IsFat, InnerApps);

        public override string[] GetKeys() => Keys;

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
