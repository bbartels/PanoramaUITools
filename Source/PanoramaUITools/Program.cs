﻿using System;
using System.Threading.Tasks;
using CommandLine;

using PanoramaUITools.Panorama;

namespace PanoramaUITools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<GenerateArchiveOptions, ExtractArchiveOptions, ModifyDllOptions>(args)
                .MapResult(
                (GenerateArchiveOptions opts) => GenerateArchive(opts),
                (ExtractArchiveOptions opts) => ExtractArchive(opts),
                (ModifyDllOptions opts) => ModifyDll(opts), errors => Task.FromResult(0));

            async Task ModifyDll(ModifyDllOptions opts)
            {
                var (success, msg) = await DllModifier.ModifyDll(opts.CsgoDirectory);
                if (!success) { Console.WriteLine(msg); }
            }

            async Task GenerateArchive(GenerateArchiveOptions opts)
            {
                var (success, msg) = await ArchiveGenerator.GenerateArchive(opts.DirectoryPath, opts.OutputPath);
                if (!success) { Console.WriteLine(msg); }
            }

            async Task ExtractArchive(ExtractArchiveOptions opts)
            {
                var (success, msg) = await ArchiveExtractor.ExtractArchive(opts.DirectoryPath, opts.OutputPath);
                if (!success) { Console.WriteLine(msg); }
            }
        }
    }
}
