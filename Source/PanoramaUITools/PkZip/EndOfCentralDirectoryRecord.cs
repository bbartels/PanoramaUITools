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

        private static readonly byte[] Signature = { 0x50, 0x4B, 0x05, 0x06 };

        public Memory<byte> EOCDHeader { get; }

        public EndOfCentralDirectoryRecord(uint centralDirOffset, uint centralDirSize, ushort totalEntries, string comment)
        {
            EOCDHeader = new byte[StaticHeaderSize + comment.Length];

            Signature.AsSpan().TryCopyTo(EOCDHeader.Span.Slice(SignatureOffset, SignatureSize));

            EOCDHeader.Span.Slice(CentralDirectorySizeOffset, CentralDirectorySizeSize).Write(centralDirSize);

            EOCDHeader.Span.Slice(CentralDirectoryOffsetOffset, CentralDirectoryOffsetSize).Write(centralDirOffset);

            EOCDHeader.Span.Slice(TotalEntriesOffset, TotalEntriesSize).Write(totalEntries);
            EOCDHeader.Span.Slice(DiskEntriesOffset, DiskEntriesSize).Write(totalEntries);

            EOCDHeader.Span.Slice(CommentLengthOffset, CommentLengthSize).Write((ushort)comment.Length);

            Encoding.ASCII.GetBytes(comment, EOCDHeader.Span.Slice(CommentOffset, comment.Length));
        }
    }
}
