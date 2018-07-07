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

        private static readonly byte[] mSignature = { 0x50, 0x4B, 0x05, 0x06 };

        private Memory<byte> _eocdHeader { get; }
        public ReadOnlySpan<byte> FileHeader { get { return _eocdHeader.Span; } }

        public ReadOnlySpan<byte> Signature
        {
            get { return FileHeader.Slice(SignatureOffset, SignatureSize); }
            private set { value.TryCopyTo(_eocdHeader.Span.Slice(SignatureOffset, SignatureSize)); }
        }

        public uint CentralDirectoryOffset
        {
            get { return BitConverter.ToUInt32(FileHeader.Slice(CentralDirectoryOffsetOffset, CentralDirectoryOffsetSize)); }
            private set { BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(CentralDirectoryOffsetOffset, CentralDirectoryOffsetSize), value); }
        }

        public uint CentralDirectorySize
        {
            get { return BitConverter.ToUInt32(FileHeader.Slice(CentralDirectorySizeOffset, CentralDirectorySizeSize)); }
            private set { BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(CentralDirectorySizeOffset, CentralDirectorySizeSize), value); }
        }

        public uint TotalEntries
        {
            get { return BitConverter.ToUInt32(FileHeader.Slice(TotalEntriesOffset, TotalEntriesSize)); }
            private set { BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(TotalEntriesOffset, TotalEntriesSize), value); }
        }

        public uint DiskEntries
        {
            get { return BitConverter.ToUInt32(FileHeader.Slice(DiskEntriesOffset, DiskEntriesSize)); }
            private set { BitConverter.TryWriteBytes(_eocdHeader.Span.Slice(DiskEntriesOffset, DiskEntriesSize), value); }
        }

        public uint CommentLength
        {
            get { return (uint)Comment.Length; }
        }

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

            Signature = mSignature;
            CentralDirectoryOffset = centralDirOffset;
            CentralDirectorySize = centralDirSize;
            TotalEntries = totalEntries;
            DiskEntries = totalEntries;
            Comment = comment;
        }
    }
}
