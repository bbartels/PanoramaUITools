using System;
using System.Threading.Tasks;
using System.Configuration;
using CommandLine;

using PanoramaUITools.Panorama;

namespace PanoramaUITools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).AppSettings.Settings;

            await Parser.Default.ParseArguments<GenerateArchiveOptions, ExtractArchiveOptions, ModifyDllOptions>(args)
                .MapResult(
                (GenerateArchiveOptions opts) => GenerateArchive(opts),
                (ExtractArchiveOptions opts) => ExtractArchive(opts),
                (ModifyDllOptions opts) => ModifyDll(opts), errors => Task.FromResult(0));

            async Task ModifyDll(ModifyDllOptions opts)
            {
                var csDir = opts.CsgoDirectory ?? config["csgoDirectory"].Value;

                WriteResult(await DllModifier.ModifyDll(csDir));
            }

            async Task GenerateArchive(GenerateArchiveOptions opts)
            {
                var inputDir = opts.DirectoryPath ?? config["archiveDirectory"].Value;
                var outputDir = opts.OutputPath ?? config["outputDirectory"].Value;

                WriteResult(await ArchiveGenerator.GenerateArchive(opts.DirectoryPath, opts.OutputPath));
            }

            async Task ExtractArchive(ExtractArchiveOptions opts)
            {
                var csDir = opts.DirectoryPath ?? config["csgoDirectory"].Value;
                var outputDir = opts.OutputPath ?? config["archiveDirectory"].Value;

                WriteResult(await ArchiveExtractor.ExtractArchive(csDir, outputDir));
            }
        }

        private static void WriteResult((bool success, string msg) result)
        {
            Console.WriteLine($"{ (result.success ? "Success": "Error") }: { result.msg }");
        }
    }
}
