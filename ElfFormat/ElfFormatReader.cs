using ElfFormat.Structs;
using FormatApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace ElfFormat
{
    internal class ElfFormatReader
    {
        private readonly byte[] Magic = new byte[] { 0x7F, (byte)'E', (byte)'L', (byte)'F' };

        private readonly EndiannessAwareBinaryReader _reader;

        public ElfFormatReader(EndiannessAwareBinaryReader reader)
        {
            _reader = reader;
        }

        public ElfHeader ReadHeader()
        {
            byte[] magic = _reader.ReadBytes(4);

            if (!magic.SequenceEqual(Magic))
                throw new FormatException("Expected ELF header was not found");

            ElfClass elfClass = _reader.ParseEnumChecked<ElfClass>(_reader.ReadByte());
            ElfEndianness endianness = _reader.ParseEnumChecked<ElfEndianness>(_reader.ReadByte());

            _reader.IsPointers64Bit = elfClass == ElfClass.Bit64;
            _reader.IsStreamLittleEndian = endianness == ElfEndianness.LittleEndian;

            byte headerVersion = _reader.ReadByte();

            OsAbi abi = _reader.ParseEnumChecked<OsAbi>(_reader.ReadByte());
            byte abiVersion = _reader.ReadByte();

            _reader.Stream.Seek(7, SeekOrigin.Current);

            FileType fileType = (FileType)_reader.ReadUInt16();
            Machine machine = (Machine)_reader.ReadUInt16();

            uint elfVersion = _reader.ReadUInt32();

            ulong entryPoint = _reader.ReadPointer();
            ulong programHeaderPointer = _reader.ReadPointer();
            ulong sectionHeaderPointer = _reader.ReadPointer();
            
            uint flags = _reader.ReadUInt32();
            ushort headerSize = _reader.ReadUInt16();

            ushort programHeaderEntrySize = _reader.ReadUInt16();
            ushort programHeaderEntriesCount = _reader.ReadUInt16();

            ushort sectionsHeaderEntrySize = _reader.ReadUInt16();
            ushort sectionsHeaderEntriesCount = _reader.ReadUInt16();
            
            ushort sectionsHeaderNamesIndex = _reader.ReadUInt16();

            return new ElfHeader()
            {
                Magic = magic,
                Class = elfClass,
                Endianness = endianness,
                Version = headerVersion,
                Abi = abi,
                AbiVersion = abiVersion,
                Type = fileType,
                Machine = machine,
                ElfVersion = elfVersion,
                EntryPoint = entryPoint,
                ProgramHeaderOffset = programHeaderPointer,
                SectionHeaderOffset = sectionHeaderPointer,
                Flags = flags,
                HeaderSize = headerSize,
                ProgramHeaderEntrySize = programHeaderEntrySize,
                ProgramHeaderEntriesCount = programHeaderEntriesCount,
                SectionsHeaderEntrySize = sectionsHeaderEntrySize,
                SectionsHeaderEntriesCount = sectionsHeaderEntriesCount,
                SectionsHeaderNamesIndex = sectionsHeaderNamesIndex,                
            };
        }

        public Segment[] ReadProgramHeaders(ElfHeader elfHeader)
        {
            _reader.Stream.Seek((long)elfHeader.ProgramHeaderOffset, SeekOrigin.Begin);

            Segment[] segments = new Segment[elfHeader.ProgramHeaderEntriesCount];

            for (int i = 0; i < elfHeader.ProgramHeaderEntriesCount; i++)
            {
                SegmentType segmentType = (SegmentType)_reader.ReadUInt32();

                SegmentFlags flags = SegmentFlags.None;

                if (elfHeader.Class == ElfClass.Bit64)
                    flags = _reader.ParseFlagsEnum<SegmentFlags>(_reader.ReadUInt32());

                ulong offset = _reader.ReadPointer();
                ulong virtualAddress = _reader.ReadPointer();
                ulong physicalAddress = _reader.ReadPointer();
                ulong fileSize = _reader.ReadPointer();
                ulong memorySize = _reader.ReadPointer();

                if (elfHeader.Class == ElfClass.Bit32)
                    flags = _reader.ParseFlagsEnum<SegmentFlags>(_reader.ReadUInt32());

                ulong align = _reader.ReadPointer();

                segments[i] = new Segment()
                {
                    Type = segmentType,
                    Flags = flags,
                    Offset = offset,
                    VirtualAddress = virtualAddress,
                    PhysicalAddress = physicalAddress,
                    FileSize = fileSize,
                    MemorySize = memorySize,
                    Alignment = align,
                };
            }

            return segments;
        }

        internal string ReadInterpreterSegment(Segment interpreterSegment)
        {
            _reader.Stream.Seek((long)interpreterSegment.Offset, SeekOrigin.Begin);

            byte[] interpreterBytes = _reader.ReadBytes((int)interpreterSegment.FileSize - 1);

            return Encoding.ASCII.GetString(interpreterBytes);
        }
    }
}
