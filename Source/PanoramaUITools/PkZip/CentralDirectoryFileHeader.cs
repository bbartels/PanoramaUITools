using System;
using System.Text;

namespace PanoramaUITools.PkZip
{
    internal class CentralDirectoryFileHeader
    {
        #region Constants

        private const byte SignatureOffset = 0x00;
        private const byte SignatureSize = 0x04;

        private const byte VersionOffset = SignatureOffset + SignatureSize;
        private const byte VersionSize = 0x02;

        private const byte VersionNeededOffset = VersionOffset + VersionSize;
        private const byte VersionNeededSize = 0x02;

        private const byte FlagsOffset = VersionNeededOffset + VersionNeededSize;
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

        private const byte FileCommentLengthOffset = ExtraFieldLengthOffset + ExtraFieldLengthSize;
        private const byte FileCommentLengthSize = 0x02;

        private const byte DiskNumberOffset = FileCommentLengthOffset + FileCommentLengthSize;
        private const byte DiskNumberSize = 0x02;

        private const byte InternalAttributeOffset = DiskNumberOffset + DiskNumberSize;
        private const byte InternalAttributeSize = 0x02;

        private const byte ExternalAttributeOffset = InternalAttributeOffset + InternalAttributeSize;
        private const byte ExternalAttributeSize = 0x04;

        private const byte LocalHeaderOffsetOffset = ExternalAttributeOffset + ExternalAttributeSize;
        private const byte LocalHeaderOffsetSize = 0x04;

        private const byte FilenameOffset = LocalHeaderOffsetOffset + LocalHeaderOffsetSize;

        private const byte StaticHeaderSize = LocalHeaderOffsetOffset + LocalHeaderOffsetSize;

        #endregion Constants

        private static readonly byte[] EocdSignature = { 0x50, 0x4B, 0x01, 0x02 };
        private static readonly byte[] EocdVersion = { 0x14, 0x00 };
        private static readonly byte[] EocdVersionNeeded = { 0x0A, 0x00 };

        public static ReadOnlySpan<byte> HeaderSignature => new ReadOnlySpan<byte>(EocdSignature);

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

        public ReadOnlySpan<byte> VersionNeeded
        {
            get => FileHeader.Slice(VersionNeededOffset, VersionNeededSize);
            private set => value.CopyTo(_fileHeader.Span.Slice(VersionNeededOffset, VersionNeededSize));
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

        public ushort FilenameLength => (ushort)Filename.Length;

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
                Encoding.ASCII.GetBytes(value,_fileHeader.Span.Slice(FilenameOffset, value.Length));
            }
        }

        public uint LocalHeaderOffset
        {
            get => BitConverter.ToUInt32(_fileHeader.Span.Slice(LocalHeaderOffsetOffset, LocalHeaderOffsetSize));
            private set => BitConverter.TryWriteBytes(_fileHeader.Span.Slice(LocalHeaderOffsetOffset, LocalHeaderOffsetSize), value);
        }

        public CentralDirectoryFileHeader(string filename, uint fileSize, uint crc32, uint localHeaderOffset)
        {
            _fileHeader = new byte[StaticHeaderSize + filename.Length];

            Signature = EocdSignature;
            Version = EocdVersion;
            VersionNeeded = EocdVersion;

            Crc32 = crc32;
            CompressedSize = fileSize;
            UncompressedSize = fileSize;

            Filename = filename;

            LocalHeaderOffset = localHeaderOffset;
        }

        public CentralDirectoryFileHeader(Span<byte> data)
        {
            if (!data.StartsWith(HeaderSignature)) { throw new ArgumentException("Invalid Central Directory FileHeader!"); }

            var headerLength = StaticHeaderSize + BitConverter.ToUInt16(data.Slice(FilenameLengthOffset, FilenameLengthSize));
            _fileHeader = new byte[headerLength];

            data.Slice(0, headerLength).CopyTo(_fileHeader.Span);
        }
    }
}
