using System.Buffers.Binary;
using FormatApi;

namespace Tools
{
    /// <summary>
    /// Binary reader that can read BigEndian and LittleEndian numbers
    /// </summary>
    public class EndiannessAwareBinaryReader
    {
        /// <summary>
        /// Underlying stream
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Indicates that numbers should be read as little endian
        /// </summary>
        public bool IsStreamLittleEndian { get; set; }

        /// <summary>
        /// Indicates that pointers are 64 bit length
        /// </summary>
        public bool IsPointers64Bit { get; set; } = false;

        /// <summary>
        /// Construct EndiannessAwareBinaryReader from given stream
        /// </summary>
        /// <param name="stream">Underlying stream</param>
        /// <param name="isStreamLittleEndian">Initial endianness</param>
        public EndiannessAwareBinaryReader(Stream stream, bool isStreamLittleEndian)
        {
            Stream = stream;
            IsStreamLittleEndian = isStreamLittleEndian;
        }

        /// <summary>
        /// Read specified numer of bytes from stream
        /// </summary>
        /// <param name="length">Number of bytes</param>
        /// <returns>Array of bytes, read from stream</returns>
        /// <exception cref="FileFormatException">Exception is thrown if was read less than expected</exception>
        public byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];

            int bytesRead = Stream.Read(data, 0, data.Length);

            if (bytesRead != data.Length)
                throw new FileFormatException($"Insufficient data to read. Expected {length} bytes, got {bytesRead} bytes");

            return data;
        }

        /// <summary>
        /// Read unsigned long
        /// </summary>
        /// <returns>Unsigned long</returns>
        public ulong ReadUInt64()
        {
            byte[] data = ReadBytes(8);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(data) : BinaryPrimitives.ReadUInt64BigEndian(data);
        }

        /// <summary>
        /// Read signed long
        /// </summary>
        /// <returns>Signed long</returns>
        public long ReadInt64()
        {
            byte[] data = ReadBytes(8);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(data) : BinaryPrimitives.ReadInt64BigEndian(data);
        }

        /// <summary>
        /// Read unsigned int
        /// </summary>
        /// <returns>Unsigned int</returns>
        public uint ReadUInt32()
        {
            byte[] data = ReadBytes(4);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(data) : BinaryPrimitives.ReadUInt32BigEndian(data);
        }

        /// <summary>
        /// Read signed int
        /// </summary>
        /// <returns>Signed int</returns>
        public int ReadInt32()
        {
            byte[] data = ReadBytes(4);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(data) : BinaryPrimitives.ReadInt32BigEndian(data);
        }

        /// <summary>
        /// Read unsigned short
        /// </summary>
        /// <returns>Unsigned short</returns>
        public ushort ReadUInt16()
        {
            byte[] data = ReadBytes(2);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(data) : BinaryPrimitives.ReadUInt16BigEndian(data);
        }

        /// <summary>
        /// Read signed short
        /// </summary>
        /// <returns>Signed short</returns>
        public short ReadInt16()
        {
            byte[] data = ReadBytes(2);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(data) : BinaryPrimitives.ReadInt16BigEndian(data);
        }

        /// <summary>
        /// Read byte
        /// </summary>
        /// <returns>Byte</returns>
        public byte ReadByte()
        {
            byte[] data = ReadBytes(1);

            return data[0];
        }

        /// <summary>
        /// Read pointer. Pointer length is determined by <see cref="IsPointers64Bit"/> field
        /// </summary>
        /// <returns>Pointer. 32bit pointers are converted to 64bit value</returns>
        public ulong ReadPointer()
        {
            return IsPointers64Bit ? ReadUInt64() : ReadUInt32();
        }

        /// <summary>
        /// Decode enum and check allowed values
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Enum value</param>
        /// <returns>Parsed enum</returns>
        /// <exception cref="FileFormatException">Exception is thrown if enum value is not defined</exception>
        public T ParseEnumChecked<T>(object value) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new FileFormatException($"Error parsing enum {typeof(T).Name}. Value {value} isn't a valid enum value");

            return (T)value;
        }

        /// <summary>
        /// Parse enum with <see cref="FlagsAttribute"/>
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="value">Enum value</param>
        /// <returns>Parsed enum</returns>
        public T ParseFlagsEnum<T>(object value) where T : Enum
        {
            return (T)value;
        }

    }
}