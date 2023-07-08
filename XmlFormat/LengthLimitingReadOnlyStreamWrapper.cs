using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlFormat
{
    internal class LengthLimitingReadOnlyStreamWrapper : Stream
    {
        public Stream UnderlyingStream { get; }

        public long LengthLimit { get; }

        public override bool CanRead => UnderlyingStream.CanRead;

        public override bool CanSeek => UnderlyingStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => Math.Min(UnderlyingStream.Length, LengthLimit);

        public override long Position { get => UnderlyingStream.Position; set => UnderlyingStream.Position = value; }        

        public override void Flush()
        {
            UnderlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long bytesAvailable = LengthLimit - UnderlyingStream.Position;

            if (bytesAvailable <= 0)
                return 0;

            int bytesToRead = (int)Math.Min(bytesAvailable, count);

            return UnderlyingStream.Read(buffer, offset, bytesToRead);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return UnderlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public LengthLimitingReadOnlyStreamWrapper(Stream underlyingStream, long lengthLimit)
        {
            UnderlyingStream = underlyingStream;
            LengthLimit = lengthLimit;
        }
    }
}
