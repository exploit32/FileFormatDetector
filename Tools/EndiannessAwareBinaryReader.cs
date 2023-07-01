using System.Buffers.Binary;
using System.Text;

namespace Tools
{
    public class EndiannessAwareBinaryReader
    {
        public Stream Stream { get; }

        public bool IsStreamLittleEndian { get; set; }

        public bool IsPointers64Bit { get; set; } = false;

        public EndiannessAwareBinaryReader(Stream stream, bool isStreamLittleEndian)
        {
            Stream = stream;
            IsStreamLittleEndian = isStreamLittleEndian;
        }

        public byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];

            try
            {
                int bytesRead = Stream.Read(data, 0, data.Length);

                if (bytesRead != data.Length)
                    throw new FormatException($"Insufficient data to read. Expected {length} bytes, got {bytesRead} bytes");
            }
            catch (IOException ex)
            {
                throw new FormatException($"Error reading data: {ex.Message}", ex);
            }

            return data;
        }

        public ulong ReadUInt64()
        {
            byte[] data = ReadBytes(8);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(data) : BinaryPrimitives.ReadUInt64BigEndian(data);
        }

        public long ReadInt64()
        {
            byte[] data = ReadBytes(8);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(data) : BinaryPrimitives.ReadInt64BigEndian(data);
        }

        public uint ReadUInt32()
        {
            byte[] data = ReadBytes(4);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(data) : BinaryPrimitives.ReadUInt32BigEndian(data);
        }

        public int ReadInt32()
        {
            byte[] data = ReadBytes(4);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(data) : BinaryPrimitives.ReadInt32BigEndian(data);
        }

        public ushort ReadUInt16()
        {
            byte[] data = ReadBytes(2);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(data) : BinaryPrimitives.ReadUInt16BigEndian(data);
        }

        public short ReadInt16()
        {
            byte[] data = ReadBytes(2);

            return IsStreamLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(data) : BinaryPrimitives.ReadInt16BigEndian(data);
        }

        public byte ReadByte()
        {
            byte[] data = ReadBytes(1);

            return data[0];
        }

        public ulong ReadPointer()
        {
            return IsPointers64Bit ? ReadUInt64() : ReadUInt32();
        }

        public T ParseEnumChecked<T>(object value) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new FormatException($"Error parsing enum {typeof(T).Name}. Value {value} isn't a valid enum value");

            return (T)value;
        }

        public T ParseFlagsEnum<T>(object value) where T : Enum
        {
            return (T)value;
        }

    }
}