using CommandLine;

namespace PanoramaUITools
{
    [Verb("generate", HelpText = "Generates Panorama Archive")]
    internal class GenerateArchiveOptions
    {
        [Option('i', "Input path for the panorama directory [Config Override]")]
        public string DirectoryPath { get; set; }

        [Option('o', "Output path for generated Archive [Config Override]")]
        public string OutputPath { get; set; }
    }

    [Verb("modify", HelpText = "Modifies panorama.dll to bypass sig checking")]
    internal class ModifyDllOptions
    {
        [Option('d', "Specify CS:GO Directory [Config Override]")]
        public string CsgoDirectory { get; set; }
    }

    [Verb("extract", HelpText = "Extracts Panorama Archive")]
    internal class ExtractArchiveOptions
    {
        [Option('i', "Input path for the input archive [Config Override]")]
        public string DirectoryPath { get; set; }

        [Option('o', "Output path for the extracted archive [Config Override]")]
        public string OutputPath { get; set; }
    }
}
