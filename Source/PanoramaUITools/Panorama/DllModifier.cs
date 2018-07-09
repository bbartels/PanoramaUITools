using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PanoramaUITools.Panorama
{
    public class DllModifier
    {
        private const string DllName = "panorama.dll";
        private static readonly Dictionary<string, (PanoramaDllParams parameters, string hash)> PanSigRefDict = new Dictionary<string, (PanoramaDllParams, string)>
        {
            { "FBBB14671FB2AD01C7E581E30A859D87", (new PanoramaDllParams (new byte[] { 0x68, 0xD4, 0xB1, 0x20, 0x10 }, -11, 0xEB), "4C4C01B61F786E4C56F247BE10E29369") },
        };

        public static async Task<(bool success, string msg)> ModifyDll(string csDir)
        {
            var dir = csDir + @"\bin\";
            var dllLocation = dir + DllName;

            if (!File.Exists(dllLocation)) { return (false, $"File: ({ dllLocation }) does not exist."); }

            File.Copy(dllLocation, dllLocation + ".bak", true);

            string hash;

            using (var md5 = MD5.Create())
            {
                hash = BitConverter.ToString(md5.ComputeHash(await File.ReadAllBytesAsync(dllLocation))).Replace("-", string.Empty);
            }

            if (PanSigRefDict.Values.Any(v => v.hash == hash)) { return (false, "The library has already been patched!"); }

            if (!PanSigRefDict.TryGetValue(hash, out var param))
            {
                return (false, $"The { DllName } you want to modify is not contained in the modification database yet.\n" +
                    "Make sure the library you want to modify is an original valve library or check for a newer version of this program.");
            }

            return await ModifyDll(dllLocation, param.parameters);
        }

        private static Task<(bool, string)> ModifyDll(string path, PanoramaDllParams modParams)
        {
            using (var file = MemoryMappedFile.CreateFromFile(path))
            {
                using (var accessor = file.CreateViewStream())
                {
                    var found = false;
                    var counter = 0;
                    int currentByte;

                    while (!found && (currentByte = accessor.ReadByte()) != -1)
                    {
                        if (modParams.ResourceReference[counter] == (byte)currentByte) { counter++; }
                        else { counter = 0; }

                        if (counter == modParams.ResourceReference.Length) { found = true; }
                    }

                    if (!found) { return Task.FromResult((false, "Error while patching DLL!")); }

                    accessor.Seek(modParams.ResourceReferenceOffset, SeekOrigin.Current);
                    accessor.WriteByte(modParams.SignatureCheckJumpInstr);

                    return Task.FromResult((true, "DLL successfully patched!"));
                }
            }
        }
    }
}
