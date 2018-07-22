using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PanoramaUITools.Panorama
{
    public class CSInstallationRestorer
    {
        public async Task<(bool success, string msg)> RestoreCSInstallation(string csPath)
        {
            RestoreDefaultPanoramaDll(csPath);
        }

        private (bool success, string msg) RestoreToDefaultUI(string csPath)
        {
        }

        private (bool success, string msg) RestoreDefaultPanoramaDll(string sourceFile, string destFile)
        {
            if (!File.Exists(destFile))
            {
                return (false, "No panorama binary exists in the specified Directory!");
            }

            if (!File.Exists(sourceFile))
            {
                return (false, "No panorama binary backup file found! Please verify your steam installation to restore defaults!");
            }

            try
            {
                File.Copy(destFile, sourceFile, true);
                File.Delete(destFile);
            }

            catch (FileNotFoundException)
            {
                return (false, "Don't know how you managed to do this!");
            }

            return (true, "");
        }
    }
}
