using System.IO;
using System.Threading.Tasks;

using PanoramaUITools.PkZip;

namespace PanoramaUITools.Panorama
{
    public class ArchiveExtractor
    {
        public static async Task<(bool success, string msg)> ExtractArchive(string inputPath, string outputPath)
        {
            if (!File.Exists(inputPath)) { return (false, "File could not be found!"); }
            if (Directory.Exists(outputPath)) { return (false, "Output directory exists already, extraction terminated!"); }

            return await PkZipArchive.ExtractFromArchive(inputPath, outputPath);
        }
    }
}
