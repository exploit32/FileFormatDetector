﻿using PEFormat.Structs;
using Tools;
using FormatApi;

namespace PEFormat
{
    /// <summary>
    /// Class that reads PE format structures
    /// </summary>
    internal class PEFormatReader
    {
        private readonly byte[] _dosSignature = new byte[] { (byte)'M', (byte)'Z' };

        private readonly byte[] _peSignature = new byte[] { (byte)'P', (byte)'E', 0, 0 };
        
        private readonly EndiannessAwareBinaryReader _reader;

        public PEFormatReader(EndiannessAwareBinaryReader reader)
        {
            this._reader = reader;
        }        

        public DosHeader ReadDosHeader()
        {
            byte[] signature = _reader.ReadBytes(_dosSignature.Length);

            if (!signature.SequenceEqual(_dosSignature))
                throw new FileFormatException("Expected MZ header was not found");

            _reader.Stream.Seek(0x3C, SeekOrigin.Begin);
            uint coffOffset = _reader.ReadUInt32();

            if (coffOffset >= _reader.Stream.Length)
                throw new FileFormatException("COFF offset points outside the file");

            return new DosHeader()
            {
                MagicNumber = signature,
                PEHeaderOffset = coffOffset,
            };
        }

        public COFFHeader ReadCOFFHeader(DosHeader dosHeader)
        {
            _reader.Stream.Seek(dosHeader.PEHeaderOffset, SeekOrigin.Begin);

            byte[] signature = _reader.ReadBytes(_peSignature.Length);

            if (!signature.SequenceEqual(_peSignature))
                throw new FileFormatException("Expected PE header was not found");

            return new COFFHeader()
            {
                Signature = signature,
                Machine = _reader.ParseEnumChecked<Machine>(_reader.ReadUInt16()),
                SectionCount = _reader.ReadUInt16(),
                TimeDataStamp = _reader.ReadUInt32(),
                PointerToSymbolTable = _reader.ReadUInt32(),
                NumberOfSymbols = _reader.ReadUInt32(),
                SizeOfOptionalHeader = _reader.ReadUInt16(),
                Characteristics = _reader.ParseFlagsEnum<Characteristics>(_reader.ReadUInt16()),
            };
        }

        public OptionalHeader? ReadOptionalHeader(COFFHeader coffHeader)
        {
            if (coffHeader.SizeOfOptionalHeader == 0)
                return null;

            OptionalHeader.PEFormat format = _reader.ParseEnumChecked<OptionalHeader.PEFormat>(_reader.ReadUInt16());

            OptionalHeader header = new OptionalHeader()
            {
                Magic = format,
                MajorLinkerVersion = _reader.ReadByte(),
                MinorLinkerVersion = _reader.ReadByte(),
                SizeOfCode = _reader.ReadUInt32(),
                SizeOfInitializedData = _reader.ReadUInt32(),
                SizeOfUninitializedData = _reader.ReadUInt32(),
                AddressOfEntryPoint = _reader.ReadUInt32(),
                BaseOfCode = _reader.ReadUInt32(),
                BaseOfData = format switch
                {
                    OptionalHeader.PEFormat.PE32 => _reader.ReadUInt32(),
                    OptionalHeader.PEFormat.PE32Plus => 0,
                    _ => throw new NotSupportedException($"Format type {format} is not supported"),
                },
                ImageBase = format switch
                {
                    OptionalHeader.PEFormat.PE32 => _reader.ReadUInt32(),
                    OptionalHeader.PEFormat.PE32Plus => _reader.ReadUInt64(),
                    _ => throw new NotSupportedException($"Format type {format} is not supported"),
                },
                SectionAlignment = _reader.ReadUInt32(),
                FileAlignment = _reader.ReadUInt32(),
                MajorOperatingSystemVersion = _reader.ReadUInt16(),
                MinorOperatingSystemVersion = _reader.ReadUInt16(),
                MajorImageVersion = _reader.ReadUInt16(),
                MinorImageVersion = _reader.ReadUInt16(),
                MajorSubsystemVersion = _reader.ReadUInt16(),
                MinorSubsystemVersion = _reader.ReadUInt16(),
                Win32VersionValue = _reader.ReadUInt32(),
                SizeOfImage = _reader.ReadUInt32(),
                SizeOfHeaders = _reader.ReadUInt32(),
                CheckSum = _reader.ReadUInt32(),
                Subsystem = _reader.ReadUInt16(),
                DllCharacteristics = _reader.ReadUInt16(),
                SizeOfStackReserve = format switch
                {
                    OptionalHeader.PEFormat.PE32 => _reader.ReadUInt32(),
                    OptionalHeader.PEFormat.PE32Plus => _reader.ReadUInt64(),
                    _ => throw new NotSupportedException($"Format type {format} is not supported"),
                },
                SizeOfStackCommit = format switch
                {
                    OptionalHeader.PEFormat.PE32 => _reader.ReadUInt32(),
                    OptionalHeader.PEFormat.PE32Plus => _reader.ReadUInt64(),
                    _ => throw new NotSupportedException($"Format type {format} is not supported"),
                },
                SizeOfHeapReserve = format switch
                {
                    OptionalHeader.PEFormat.PE32 => _reader.ReadUInt32(),
                    OptionalHeader.PEFormat.PE32Plus => _reader.ReadUInt64(),
                    _ => throw new NotSupportedException($"Format type {format} is not supported"),
                },
                SizeOfHeapCommit = format switch
                {
                    OptionalHeader.PEFormat.PE32 => _reader.ReadUInt32(),
                    OptionalHeader.PEFormat.PE32Plus => _reader.ReadUInt64(),
                    _ => throw new NotSupportedException($"Format type {format} is not supported"),
                },
                LoaderFlags = _reader.ReadUInt32(),
                NumberOfRvaAndSizes = _reader.ReadUInt32(),                
            };

            header.DataDirectories = new DataDirectory[header.NumberOfRvaAndSizes];

            for (int i = 0; i < header.NumberOfRvaAndSizes; i++)
            {
                header.DataDirectories[i] = new DataDirectory()
                {
                    RVA = _reader.ReadUInt32(),
                    Size = _reader.ReadUInt32(),
                };
            }

            return header;
        }
    }
}
