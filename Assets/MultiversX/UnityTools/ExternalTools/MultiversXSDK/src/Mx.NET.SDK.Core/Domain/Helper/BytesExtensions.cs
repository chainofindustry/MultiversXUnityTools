using System;

namespace Mx.NET.SDK.Core.Domain.Helper
{
    public static class BytesExtensions
    {
        public static byte[] Slice(this byte[] source, int start, int? optEnd = null)
        {
            var end = optEnd.GetValueOrDefault(source.Length);
            var len = end - start;

            // Return new array.
            var res = new byte[len];
            for (var i = 0; i < len; i++) res[i] = source[i + start];
            return res;
        }

        public static int ReadUInt32BE(this byte[] buffer, int offset)
        {
            if (buffer.Length < offset + 4)
            {
                throw new ArgumentException("Buffer is too short to read UInt32BE at the specified offset.");
            }

            return (buffer[offset] << 24) | (buffer[offset + 1] << 16) | (buffer[offset + 2] << 8) | buffer[offset + 3];
        }

        public static void WriteUInt32BE(this byte[] buffer, int value, int offset = 0)
        {
            if (buffer.Length < offset + 4)
            {
                throw new ArgumentException("Buffer is too short to write UInt32BE at the specified offset.");
            }

            buffer[offset] = (byte)((value >> 24) & 0xFF); // Write the most significant byte
            buffer[offset + 1] = (byte)((value >> 16) & 0xFF); // Write the second most significant byte
            buffer[offset + 2] = (byte)((value >> 8) & 0xFF);  // Write the second least significant byte
            buffer[offset + 3] = (byte)(value & 0xFF); // Write the least significant byte
        }
    }
}
