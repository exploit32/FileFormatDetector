using FormatApi;
using MachOFormat.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace MachOFormat
{
    internal class MachOFormatReader
    {
        public static readonly Signature Magic = new Signature(new byte[] { 0xfe, 0xed, 0xfa, 0xce });
        public static readonly Signature Cigam = new Signature(new byte[] { 0xce, 0xfa, 0xed, 0xfe });
        public static readonly Signature Magic64 = new Signature(new byte[] { 0xfe, 0xed, 0xfa, 0xcf });
        public static readonly Signature Cigam64 = new Signature(new byte[] { 0xcf, 0xfa, 0xed, 0xfe });

        //FAT Magics: https://opensource.apple.com/source/cctools/cctools-895/include/mach-o/fat.h.auto.html
        public static readonly Signature FatMagic = new Signature(new byte[] { 0xca, 0xfe, 0xba, 0xbe });
        public static readonly Signature FatMagic64 = new Signature(new byte[] { 0xca, 0xfe, 0xba, 0xbf });

        private readonly EndiannessAwareBinaryReader _reader;

        public MachOFormatReader(EndiannessAwareBinaryReader reader)
        {
            _reader = reader;
        }

        public MachOMagic ReadMagic()
        {
            byte[] magic = _reader.ReadBytes(4);
            bool isFat = false;

            int bits;
            Endianness endianness;
            if (SignatureTools.CheckSignature(magic, Magic))
            {
                bits = 32;
                endianness = Endianness.BigEndian;
            }
            else if (SignatureTools.CheckSignature(magic, Cigam))
            {
                bits = 32;
                endianness = Endianness.LittleEndian;
            }
            else if (SignatureTools.CheckSignature(magic, Magic64))
            {
                bits = 64;
                endianness = Endianness.BigEndian;
            }
            else if (SignatureTools.CheckSignature(magic, Cigam64))
            {
                bits = 64;
                endianness = Endianness.LittleEndian;
            }
            else if (SignatureTools.CheckSignature(magic, FatMagic))
            {
                bits = 32;
                endianness = Endianness.BigEndian;
                isFat = true;
            }
            else if (SignatureTools.CheckSignature(magic, FatMagic64))
            {
                bits = 64;
                endianness = Endianness.BigEndian;
                isFat = true;
            }
            else
            {
                throw new FileFormatException("Expected Mach-O header was not found");
            }

            return new MachOMagic()
            {
                Bits = bits,
                Endianness = endianness,
                IsFat = isFat,
            };
        }

        public MachO ReadMachO()
        {
            MachO result;

            var magic = ReadMagic();            

            if (magic.IsFat)
            {
                FatHeader fatHeader = ReadFatHeader(magic);

                MachO[] innerMachOs = new MachO[fatHeader.Archs.Length];

                for (int i = 0; i < fatHeader.Archs.Length; i++)
                {
                    var arch = fatHeader.Archs[i];

                    _reader.Stream.Seek((long)arch.Offset, SeekOrigin.Begin);

                    innerMachOs[i] = ReadMachO();
                }

                result = new MachO()
                {
                    Magic = magic,
                    IsSigned = false,
                    InnerMachOs = innerMachOs,
                };
            }
            else
            {
                var appHeader = ReadAppHeader(magic);
                var hasSignature = FindCodeSignature(appHeader);

                result = new MachO()
                {
                    Magic = magic,
                    IsSigned = hasSignature,
                    Architecture = appHeader.CpuType.ToString(),
                };
            }

            return result;
        }

        private AppHeader ReadAppHeader(MachOMagic machOFileSumary)
        {
            _reader.IsPointers64Bit = machOFileSumary.Bits == 64;
            _reader.IsStreamLittleEndian = machOFileSumary.Endianness == Endianness.LittleEndian;

            var header = new AppHeader()
            {
                CpuType = (CpuType)_reader.ReadUInt32(),
                CpuSubtype = _reader.ReadUInt32(),
                FileType = _reader.ReadUInt32(),
                NumberOfLoadCommands = _reader.ReadUInt32(),
                SizeOfLoadCommands = _reader.ReadUInt32(),
                Flags = _reader.ReadUInt32(),
            };

            if (_reader.IsPointers64Bit)
                _reader.Stream.Seek(4, SeekOrigin.Current);

            return header;
        }

        private FatHeader ReadFatHeader(MachOMagic machOFileSumary)
        {
            _reader.IsPointers64Bit = machOFileSumary.Bits == 64;
            _reader.IsStreamLittleEndian = machOFileSumary.Endianness == Endianness.LittleEndian;

            uint nFatArch = _reader.ReadUInt32();
            FatArch[] archs = new FatArch[nFatArch];

            for (uint i = 0; i < nFatArch; i++)
            {
                CpuType cpuType = (CpuType)_reader.ReadUInt32();
                uint cpuSubtype = _reader.ReadUInt32();

                ulong offset = _reader.ReadPointer();
                ulong size = _reader.ReadPointer();
                uint align = _reader.ReadUInt32();

                if (machOFileSumary.Bits == 64)
                {
                    _reader.Stream.Seek(4, SeekOrigin.Current);
                }

                archs[i] = new FatArch()
                {
                    CpuType = cpuType,
                    CpuSubtype = cpuSubtype,
                    Offset = offset,
                    Size = size,
                    Align = align,
                };
            }

            return new FatHeader()
            {
                NumberOfArchs = nFatArch,
                Archs = archs,
            };
        }

        private bool FindCodeSignature(AppHeader appHeader)
        {
            bool signatureFound = false;
            uint totalLoadCommandsSize = 0;

            for (int i = 0; i < appHeader.NumberOfLoadCommands; i++)
            {
                var commandType = (LoadCommandType)_reader.ReadUInt32();
                uint size = _reader.ReadUInt32();

                totalLoadCommandsSize += size;
                if (totalLoadCommandsSize > appHeader.SizeOfLoadCommands)
                    throw new FileFormatException($"Index of load commands table ({totalLoadCommandsSize}) exceeded table size from header ({appHeader.SizeOfLoadCommands} bytes)");

                if (commandType == LoadCommandType.LC_CODE_SIGNATURE)
                {
                    signatureFound = true;
                    break;
                }

                //Skipping load command
                _reader.Stream.Seek(size - MachOConsts.LoadCommandHeaderSize, SeekOrigin.Current);
            }

            return signatureFound;
        }
    }
}
