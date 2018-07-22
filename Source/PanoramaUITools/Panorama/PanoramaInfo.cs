using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PanoramaUITools.Panorama
{
    internal static class PanoramaInfo
    {
        private const string BinaryName = "panorama.dll";
        private const string BackupExtension = ".bak.put";
        private const string PanoramaBinDir = @"/bin/";

        public static string GetPanoramaBinDir(string csDir)
        {
            return Path.Combine(csDir, PanoramaBinDir);
        }

        public static string GetPanoramaBinLocation(string csDir)
        {
            return Path.Combine("", "");
        }

        public static string GetPanoramaBinBackupLocation(string csDir)
        {
            return Path.Combine("", "");
        }

        public static string GetPanoramaUILocation(string csDir)
        {
            return Path.Combine("", "");
        }

        public static string GetPanoramaUIBackupLocation(string csDir)
        {
            return Path.Combine("", "");
        }
    }
}
