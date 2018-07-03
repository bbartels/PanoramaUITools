using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using PanoramaUITools.PkZip;

namespace PanoramaUITools.Panorama
{
    internal static class ArchiveGenerator
    {
        private const string PanoramaFilename = @"code.pbin";

        private const uint PanHeaderLength = 516;
        private static readonly byte[] PanHeader = { 0x50, 0x41, 0x4E, 0x01 };
        private static readonly byte[] PanTrailer = { 0x01 };
        private static readonly string PanComment = "XZP1 0";
        private static readonly List<string> Files = new List<string>();

        public static async Task<(bool success, string msg)> GenerateArchive(string inputPath, string outputPath)
        {
            if (!Directory.Exists(inputPath)) { return (false, "Directory to be archived does not exist!"); }

            var outputFilePath = outputPath + (outputPath.Last() == '\\' ? string.Empty : "\\") + PanoramaFilename;

            var archive = new PkZipArchive(PanComment);

            var pathSplitIndex = inputPath.LastIndexOf(@"\") + 1;

            RecDirSearch(inputPath);

            foreach (var file in Files)
            {
                archive.AddFile(new PkZipFile(file.Substring(pathSplitIndex), await File.ReadAllBytesAsync(file)));
            }

            var header = PanHeader.Concat(new byte[PanHeaderLength - PanHeader.Length]).ToArray();
            archive.GenerateArchive(header, PanTrailer, outputPath + PanoramaFilename);
            return (true, "Archive generated successfully!");
        }

        private static void RecDirSearch(string dir)
        {
            try
            {
                Files.AddRange(Directory.GetFiles(dir));

                foreach (string directory in Directory.GetDirectories(dir))
                {
                    RecDirSearch(directory);
                }
            }

            catch (Exception) { }
        }
    }
}
