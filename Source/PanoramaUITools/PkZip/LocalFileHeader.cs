using System;
using System.Text;

namespace PanoramaUITools.PkZip
{
    internal sealed class LocalFileHeader
    {
        private static readonly byte[] Signature = { 0x50, 0x4B, 0x03, 0x04 };
        private static readonly byte[] Version = { 0x0A, 0x00 };

        private const byte SignatureOffset = 0x00;
        private const byte SignatureSize = 0x04;

        private const byte VersionOffset = SignatureOffset + SignatureSize;
        private const byte VersionSize = 0x02;

        private const byte FlagsOffset = VersionOffset + VersionSize;
        private const byte FlagsSize = 0x02;

        private const byte CompressionOffset = FlagsOffset + FlagsSize;
        private const byte CompressionSize = 0x02;

        private const byte ModificationTimeOffset = CompressionOffset + CompressionSize;
        private const byte ModificationTimeSize = 0x02;

        private const byte ModificationDateOffset = ModificationTimeOffset + ModificationTimeSize;
        private const byte ModificationDateSize = 0x02;

        private const byte Crc32Offset = ModificationDateOffset + ModificationDateSize;
        private const byte Crc32Size = 0x04;

        private const byte CompressedSizeOffset = Crc32Offset + Crc32Size;
        private const byte CompressedSizeSize = 0x04;

        private const byte UncompressedSizeOffset = CompressedSizeOffset + CompressedSizeSize;
        private const byte UncompressedSizeSize = 0x04;

        private const byte FilenameLengthOffset = UncompressedSizeOffset + UncompressedSizeSize;
        private const byte FilenameLengthSize = 0x02;

        private const byte ExtraFieldLengthOffset = FilenameLengthOffset + FilenameLengthSize;
        private const byte ExtraFieldLengthSize = 0x02;

        private const byte FilenameOffset = ExtraFieldLengthOffset + ExtraFieldLengthSize;

        private const byte StaticHeaderSize = 30;

        public Memory<byte> FileHeader { get; set; }

        public LocalFileHeader(string filename, uint fileSize, uint crc32)
        {
            FileHeader = new byte[StaticHeaderSize + filename.Length];

            Signature.AsSpan().TryCopyTo(FileHeader.Span.Slice(SignatureOffset, SignatureSize));
            Version.AsSpan().TryCopyTo(FileHeader.Span.Slice(VersionOffset, VersionSize));

            FileHeader.Span.Slice(Crc32Offset, Crc32Size).Write(crc32);

            FileHeader.Span.Slice(CompressedSizeOffset, CompressedSizeSize).Write(fileSize);
            FileHeader.Span.Slice(UncompressedSizeOffset, UncompressedSizeSize).Write(fileSize);

            FileHeader.Span.Slice(FilenameLengthOffset, FilenameLengthSize).Write((ushort)filename.Length);

            Encoding.ASCII.GetBytes(filename, FileHeader.Span.Slice(FilenameOffset, filename.Length));
        }
    }
}
