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

        private const byte StaticHeaderSize = 30;

        #endregion Constants

        private static readonly byte[] mSignature = { 0x50, 0x4B, 0x03, 0x04 };
        private static readonly byte[] mVersion = { 0x0A, 0x00 };

        public ReadOnlySpan<byte> Signature
        {
            get { return FileHeader.Span.Slice(SignatureOffset, SignatureSize); }
            private set { value.TryCopyTo(FileHeader.Span.Slice(SignatureOffset, SignatureSize)); }
        }

        public ReadOnlySpan<byte> Version
        {
            get { return FileHeader.Span.Slice(VersionOffset, VersionSize); }
            private set { value.TryCopyTo(FileHeader.Span.Slice(VersionOffset, VersionSize)); }
        }

        public uint Crc32
        {
            get { return BitConverter.ToUInt32(FileHeader.Span.Slice(Crc32Offset, Crc32Size)); }
            private set { BitConverter.TryWriteBytes(FileHeader.Span.Slice(Crc32Offset, Crc32Size), value); }
        }

        public uint CompressedSize
        {
            get { return BitConverter.ToUInt32(FileHeader.Span.Slice(CompressedSizeOffset, CompressedSizeSize)); }
            private set { BitConverter.TryWriteBytes(FileHeader.Span.Slice(CompressedSizeOffset, CompressedSizeSize), value); }
        }

        public uint UncompressedSize
        {
            get { return BitConverter.ToUInt32(FileHeader.Span.Slice(UncompressedSizeOffset, UncompressedSizeSize)); }
            private set { BitConverter.TryWriteBytes(FileHeader.Span.Slice(UncompressedSizeOffset, UncompressedSizeSize), value); }
        }
        public uint FilenameLength
        {
            get { return (uint)Filename.Length; }
        }

        public string Filename
        {
            get
            {
                var len = (int)BitConverter.ToUInt32(FileHeader.Span.Slice(FilenameLengthOffset, FilenameLengthSize));
                return Encoding.ASCII.GetString(FileHeader.Span.Slice(FilenameOffset, len));
            }
            private set
            {
                BitConverter.TryWriteBytes(FileHeader.Span.Slice(FilenameLengthOffset, FilenameLengthSize), value.Length);
                Encoding.ASCII.GetBytes(value, FileHeader.Span.Slice(FilenameOffset, value.Length));
            }
        }

        public Memory<byte> FileHeader { get; set; }

        public LocalFileHeader(string filename, uint fileSize, uint crc32)
        {
            FileHeader = new byte[StaticHeaderSize + filename.Length];

            Signature = mSignature;
            Version = mVersion;

            Crc32 = crc32;
            CompressedSize = fileSize;
            UncompressedSize = fileSize;

            Filename = filename;
        }
    }
}
