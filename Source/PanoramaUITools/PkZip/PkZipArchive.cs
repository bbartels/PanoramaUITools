using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PanoramaUITools.PkZip
{
    internal class PkZipArchive
    {
        private readonly List<PkZipFile> _files = new List<PkZipFile>();
        private readonly string _comment;

        public PkZipArchive(string comment)
        {
            _comment = comment;
        }

        public void AddFile(PkZipFile file)
        {
            _files.Add(file);
        }

        public static async Task ExtractFromArchive(string inputPath, string outputPath)
        {
            using (var fileStream = new FileStream(inputPath, FileMode.Open))
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    var test = new CentralDirectory(reader.BaseStream);
                    await test.ExtractAll(Path.Combine(outputPath, Path.GetFileNameWithoutExtension(inputPath)), new System.Threading.CancellationToken());
                }
            }
        }

        public void GenerateArchive(byte[] header, byte[] trailer, string outputPath)
        {
            var headerLength = header.Length;

            using (var memory = new FileStream(outputPath, FileMode.Create))
            {
                using (var writer = new BinaryWriter(memory))
                {
                    writer.Write(header);

                    foreach (var file in _files)
                    {
                        file.LocalHeaderOffset = (uint)(writer.BaseStream.Length - headerLength);
                        writer.Write(file.LocalFileHeader.FileHeader);
                        writer.Write(file.File.Span);
                    }

                    var localFilesSize = writer.BaseStream.Length - headerLength;

                    foreach (var file in _files)
                    {
                        writer.Write(file.CentralDircetoryFileHeader.FileHeader);
                    }

                    var centralDirSize = writer.BaseStream.Length - localFilesSize - headerLength;

                    var eocdRecord = new EndOfCentralDirectoryRecord((uint)localFilesSize, (uint)centralDirSize, (ushort)_files.Count, _comment);

                    writer.Write(eocdRecord.FileHeader);
                    writer.Write(trailer);
                }
            }
        }
    }
}
