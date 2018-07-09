using System;
using Force.Crc32;

namespace PanoramaUITools.PkZip
{
    internal class PkZipFile
    {
        public LocalFileHeader LocalFileHeader { get; }
        public uint LocalHeaderOffset { get; set; }

        public CentralDirectoryFileHeader CentralDircetoryFileHeader => new CentralDirectoryFileHeader(Filename, (uint)File.Length, Crc32, LocalHeaderOffset);

        private readonly Memory<byte> _file;

        public ReadOnlyMemory<byte> File => _file;
        public string Filename => LocalFileHeader.Filename;

        private uint? _crc32;
        public uint Crc32
        {
            get
            {
                if (_crc32 == null) { _crc32 = Crc32Algorithm.Compute(File.ToArray()); }
                return (uint)_crc32;
            }
        }

        public PkZipFile(string filename, byte[] file)
        {
            _file = file;

            LocalFileHeader = new LocalFileHeader(filename, (uint)File.Length, Crc32);
        }

        internal PkZipFile(LocalFileHeader header, uint headerOffset, Memory<byte> file)
        {
            _file = file;

            LocalFileHeader = header;
            LocalHeaderOffset = headerOffset;
        }
    }
}
