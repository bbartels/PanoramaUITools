using System.Collections.Generic;
using System.IO;

namespace PanoramaUITools.PkZip
{
    internal class PkZipArchive
    {
        private List<PkZipFile> _files = new List<PkZipFile>();
        private string _comment { get; set; }

        public PkZipArchive(string comment)
        {
            _comment = comment;
        }

        public void AddFile(PkZipFile file)
        {
            _files.Add(file);
        }

        public void GenerateArchive(byte[] header, byte[] trailer, string outputPath)
        {
            var headerLength = header.Length;
            long localFilesSize = 0;
            long centralDirSize = 0;

            using (var memory = new FileStream(outputPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(header);

                    foreach (var file in _files)
                    {
                        file.LocalHeaderOffset = (uint)(writer.BaseStream.Length - headerLength);
                        writer.Write(file.LocalFileHeader.FileHeader.Span);
                        writer.Write(file.File.Span);
                    }

                    localFilesSize = writer.BaseStream.Length - headerLength;

                    foreach (var file in _files)
                    {
                        writer.Write(file.CentralDircetoryFileHeader.FileHeader.Span);
                    }

                    centralDirSize = writer.BaseStream.Length - localFilesSize - headerLength;

                    var eocdRecord = new EndOfCentralDirectoryRecord((uint)localFilesSize, (uint)centralDirSize, (ushort)_files.Count, _comment);

                    writer.Write(eocdRecord.EOCDHeader.Span);
                    writer.Write(trailer);
                }
            }
        }
    }
}
