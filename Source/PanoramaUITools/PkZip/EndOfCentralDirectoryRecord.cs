using System;
using System.Text;

namespace PanoramaUITools.PkZip
{
    internal class EndOfCentralDirectoryRecord
    {
        #region Constants

        private const byte SignatureOffset = 0x00;
        private const byte SignatureSize = 0x04;

        private const byte DiskNumberOffset = SignatureOffset + SignatureSize;
        private const byte DiskNumberSize = 0x02;

        private const byte CentralDirDiskNumOffset = DiskNumberOffset + DiskNumberSize;
        private const byte CentralDirDiskSize = 0x02;

        private const byte DiskEntriesOffset = CentralDirDiskNumOffset + CentralDirDiskSize;
        private const byte DiskEntriesSize = 0x02;

        private const byte TotalEntriesOffset = DiskEntriesOffset + DiskEntriesSize;
        private const byte TotalEntriesSize = 0x02;

        private const byte CentralDirectorySizeOffset = TotalEntriesOffset + TotalEntriesSize;
        private const byte CentralDirectorySizeSize = 0x04;

        private const byte CentralDirectoryOffsetOffset = CentralDirectorySizeOffset + CentralDirectorySizeSize;
        private const byte CentralDirectoryOffsetSize = 0x04;

        private const byte CommentLengthOffset = CentralDirectoryOffsetOffset + CentralDirectoryOffsetSize;
        private const byte CommentLengthSize = 0x02;

        private const byte CommentOffset = CommentLengthOffset + CommentLengthSize;

        private const byte StaticHeaderSize = CommentLengthOffset + CommentLengthSize;

        #endregion Constants

        private static readonly byte[] EocdSignature = { 0x50, 0x4B, 0x05, 0x06 };

        public static ReadOnlySpan<byte> HeaderSignature => new ReadOnlySpan<byte>(EocdSignature);

        private readonly Memory<byte> _eocdHeader;
        public ReadOnlySpan<byte> FileHeader => _eocdHeader.Span;

        public ReadOnlySpan<byte> Signature
        {
            get => FileHeader.Slice(SignatureOffset, SignatureSize);
            private set => value.CopyTo(_eocdHeader.Span.Slice(SignatureOffset, SignatureSize));
        }

        public uint CentralDirectoryOffset
        {
            get => BitConverter.ToUInt32(FileHeader.Slice(CentralDirectoryOffsetOffset, CentralDirectoryOffsetSize));
            private set => BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(CentralDirectoryOffsetOffset, CentralDirectoryOffsetSize), value);
        }

        public uint CentralDirectorySize
        {
            get => BitConverter.ToUInt32(FileHeader.Slice(CentralDirectorySizeOffset, CentralDirectorySizeSize));
            private set => BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(CentralDirectorySizeOffset, CentralDirectorySizeSize), value);
        }

        public ushort TotalEntries
        {
            get => BitConverter.ToUInt16(FileHeader.Slice(TotalEntriesOffset, TotalEntriesSize));
            private set => BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(TotalEntriesOffset, TotalEntriesSize), value);
        }

        public ushort DiskEntries
        {
            get => BitConverter.ToUInt16(FileHeader.Slice(DiskEntriesOffset, DiskEntriesSize));
            private set => BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(DiskEntriesOffset, DiskEntriesSize), value);
        }

        public uint CommentLength => (uint)Comment.Length;

        public string Comment
        {
            get
            {
                var len = (int)BitConverter.ToUInt32(FileHeader.Slice(CommentLengthOffset, CommentLengthSize));
                return Encoding.ASCII.GetString(FileHeader.Slice(CommentOffset, len));
            }

            private set
            {
                BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(CommentLengthOffset, CommentLengthSize), value.Length);
                Encoding.ASCII.GetBytes(value, _eocdHeader.Span.Slice(CommentOffset, value.Length));
            }
        }

        public EndOfCentralDirectoryRecord(uint centralDirOffset, uint centralDirSize, ushort totalEntries, string comment)
        {
            _eocdHeader = new byte[StaticHeaderSize + comment.Length];

            Signature = HeaderSignature;
            CentralDirectoryOffset = centralDirOffset;
            CentralDirectorySize = centralDirSize;
            TotalEntries = totalEntries;
            DiskEntries = totalEntries;
            Comment = comment;
        }

        public EndOfCentralDirectoryRecord(Span<byte> data)
        {
            if (!data.StartsWith(HeaderSignature)) { throw new ArgumentException("Invalid EndofCentralDirectory Header!"); }

            var headerLength = StaticHeaderSize + BitConverter.ToUInt16(data.Slice(CommentLengthOffset, CommentLengthSize));
            _eocdHeader = new byte[headerLength];

            data.Slice(0, headerLength).CopyTo(_eocdHeader.Span);
        }
    }
}
