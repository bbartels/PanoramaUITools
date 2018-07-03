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
        private static readonly byte[] _panHeader = { 0x50, 0x41, 0x4E, 0x01 };
        private static readonly byte[] _panTrailer = { 0x01 };
        private static readonly string _panComment = "XZP1 0";
        private static readonly List<string> files = new List<string>();

        public static async Task<(bool success, string msg)> GenerateArchive(string inputPath, string outputPath)
        {
            if (!Directory.Exists(inputPath)) { return (false, "Directory to be archived does not exist!"); }

            var outputFilePath = outputPath + (outputPath.Last() == '\\' ? "" : "\\") + PanoramaFilename;

            var archive = new PkZipArchive(_panComment);

            var pathSplitIndex = inputPath.LastIndexOf(@"\") + 1;

            RecDirSearch(inputPath);

            foreach (var file in files)
            {
                archive.AddFile(new PkZipFile(file.Substring(pathSplitIndex), await File.ReadAllBytesAsync(file)));
            }

            var header = _panHeader.Concat(new byte[PanHeaderLength - _panHeader.Length]).ToArray();
            archive.GenerateArchive(header, _panTrailer, outputPath + PanoramaFilename);
            return (true, "Archive generated successfully!");
        }

        private static void RecDirSearch(string dir)
        {
            try
            {
                files.AddRange(Directory.GetFiles(dir));

                foreach (string directory in Directory.GetDirectories(dir))
                {
                    RecDirSearch(directory);
                }
            }

            catch (Exception) { }
        }
    }
}
