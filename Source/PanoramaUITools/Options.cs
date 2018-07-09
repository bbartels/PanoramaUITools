using CommandLine;

namespace PanoramaUITools
{
    [Verb("generate", HelpText = "Generates Panorama Archive")]
    internal class GenerateArchiveOptions
    {
        [Option('i', "Input path for the panorama directory", Required = true)]
        public string DirectoryPath { get; set; }

        [Option('o', "Output path for generated Archive", Required = true)]
        public string OutputPath { get; set; }
    }

    [Verb("modify", HelpText = "Modifies panorama.dll to bypass sig checking")]
    internal class ModifyDllOptions
    {
        [Option('d', "Specify CS:GO Directory", Required = true)]
        public string CsgoDirectory { get; set; }
    }

    [Verb("extract", HelpText = "Extracts Panorama Archive")]
    internal class ExtractArchiveOptions
    {
        [Option('i', "Input path for the input archive", Required = true)]
        public string DirectoryPath { get; set; }

        [Option('o', "Output path for the extracted archive")]
        public string OutputPath { get; set; }
    }
}
