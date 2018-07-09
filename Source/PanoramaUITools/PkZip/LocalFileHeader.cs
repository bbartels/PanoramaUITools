using System;
using System.Text;

namespace PanoramaUITools.PkZip
{
    internal sealed class LocalFileHeader
    {
        #region Constants

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

        public const byte StaticHeaderSize = 30;

        #endregion Constants

        private static readonly byte[] LfhSignature = { 0x50, 0x4B, 0x03, 0x04 };
        private static readonly byte[] LfhVersion = { 0x0A, 0x00 };

        private readonly Memory<byte> _fileHeader;
        public ReadOnlySpan<byte> FileHeader => _fileHeader.Span;

        public ReadOnlySpan<byte> Signature
        {
            get => FileHeader.Slice(SignatureOffset, SignatureSize);
            private set => value.CopyTo(_fileHeader.Span.Slice(SignatureOffset, SignatureSize));
        }

        public ReadOnlySpan<byte> Version
        {
            get => FileHeader.Slice(VersionOffset, VersionSize);
            private set => value.CopyTo(_fileHeader.Span.Slice(VersionOffset, VersionSize));
        }

        public uint Crc32
        {
            get => BitConverter.ToUInt32(FileHeader.Slice(Crc32Offset, Crc32Size));
            private set => BitConverter.TryWriteBytes(_fileHeader.Span.Slice(Crc32Offset, Crc32Size), value);
        }

        public uint CompressedSize
        {
            get => BitConverter.ToUInt32(FileHeader.Slice(CompressedSizeOffset, CompressedSizeSize));
            private set => BitConverter.TryWriteBytes(_fileHeader.Span.Slice(CompressedSizeOffset, CompressedSizeSize), value);
        }

        public uint UncompressedSize
        {
            get => BitConverter.ToUInt32(FileHeader.Slice(UncompressedSizeOffset, UncompressedSizeSize));
            private set => BitConverter.TryWriteBytes(_fileHeader.Span.Slice(UncompressedSizeOffset, UncompressedSizeSize), value);
        }
        public uint FilenameLength => (uint)Filename.Length;

        public string Filename
        {
            get
            {
                var len = (int)BitConverter.ToUInt16(FileHeader.Slice(FilenameLengthOffset, FilenameLengthSize));
                return Encoding.ASCII.GetString(FileHeader.Slice(FilenameOffset, len));
            }

            private set
            {
                BitConverter.TryWriteBytes(_fileHeader.Span.Slice(FilenameLengthOffset, FilenameLengthSize), value.Length);
                Encoding.ASCII.GetBytes(value, _fileHeader.Span.Slice(FilenameOffset, value.Length));
            }
        }

        public LocalFileHeader(string filename, uint fileSize, uint crc32)
        {
            _fileHeader = new byte[StaticHeaderSize + filename.Length];

            Signature = LfhSignature;
            Version = LfhVersion;

            Crc32 = crc32;
            CompressedSize = fileSize;
            UncompressedSize = fileSize;

            Filename = filename;
        }

        public LocalFileHeader(Span<byte> data)
        {
            if (!data.StartsWith(LfhSignature)) { throw new ArgumentException("Invalid LocalFileHeader!"); }

            var headerLength = StaticHeaderSize + BitConverter.ToUInt16(data.Slice(FilenameLengthOffset, FilenameLengthSize));
            _fileHeader = new byte[headerLength];

            data.Slice(0, headerLength).CopyTo(_fileHeader.Span);
        }
    }
}
