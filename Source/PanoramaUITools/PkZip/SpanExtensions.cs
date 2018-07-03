using System;

namespace PanoramaUITools.PkZip
{
    internal static class SpanExtensions
    {
        public static void Write(this Span<byte> span, uint value)
        {
            if (span.Length < 4) { throw new ArgumentException($"Parameter { nameof(span) } needs to have a length of 4 or higher!"); }
            span[0] = (byte)(value);
            span[1] = (byte)(value >> 0x08);
            span[2] = (byte)(value >> 0x10);
            span[3] = (byte)(value >> 0x18);
        }

        public static void Write(this Span<byte> span, ushort value)
        {
            if (span.Length < 2) { throw new ArgumentException($"Parameter { nameof(span) } needs to have a length of 2 or higher!"); }
            span[0] = (byte)(value);
            span[1] = (byte)(value >> 0x08);
        }
    }
}
