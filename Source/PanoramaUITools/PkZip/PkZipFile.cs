using System;
using Force.Crc32;

namespace PanoramaUITools.PkZip
{
    internal class PkZipFile
    {
        public LocalFileHeader LocalFileHeader { get; }
        public uint LocalHeaderOffset { get; set; }

        public CentralDirectoryFileHeader CentralDircetoryFileHeader
        {
            get
            {
                return new CentralDirectoryFileHeader(Filename, (uint)File.Length, Crc32, LocalHeaderOffset);
            }
        }

        public string Filename { get; private set; }
        public Memory<byte> File { get; }

        private uint? crc32 = null;
        public uint Crc32
        {
            get
            {
                if (crc32 == null) { crc32 = Crc32Algorithm.Compute(File.ToArray()); }
                return (uint)crc32;
            }
        }

        public PkZipFile(string filename, byte[] file)
        {
            File = file;
            Filename = filename;

            LocalFileHeader = new LocalFileHeader(Filename, (uint)File.Length, Crc32);
        }
    }
}
