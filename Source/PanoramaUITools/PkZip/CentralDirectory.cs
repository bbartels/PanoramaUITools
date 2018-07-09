using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PanoramaUITools.PkZip
{
    internal class CentralDirectory
    {
        private readonly Stream _stream;
        private readonly List<CentralDirectoryFileHeader> _cdHeaders;
        private readonly EndOfCentralDirectoryRecord _eocdHeader;

        public CentralDirectory(Stream reader)
        {
            _stream = reader;
            var sig = EndOfCentralDirectoryRecord.HeaderSignature;

            _stream.Seek(-1024, SeekOrigin.End);

            var found = false;
            var counter = 0;
            int currentByte;

            while (!found && (currentByte = _stream.ReadByte()) != -1)
            {
                if (sig[counter] == (byte)currentByte) { counter++; }
                else { counter = 0; }

                if (counter == sig.Length) { found = true; }
            }

            if (found)
            {
                _stream.Seek(-4, SeekOrigin.Current);

                Span<byte> buffer = stackalloc byte[(int)(_stream.Length - _stream.Position)];
                _stream.Read(buffer);
                _eocdHeader = new EndOfCentralDirectoryRecord(buffer);

                _stream.Seek(_eocdHeader.CentralDirectoryOffset + 516, SeekOrigin.Begin);

                var cdBuffer = new byte[_eocdHeader.CentralDirectorySize].AsSpan();
                _stream.Read(cdBuffer);

                _cdHeaders = new List<CentralDirectoryFileHeader>(_eocdHeader.TotalEntries);
                var offset = 0;

                while (_cdHeaders.Count < _eocdHeader.TotalEntries)
                {
                    var header = new CentralDirectoryFileHeader(cdBuffer.Slice(offset));
                    offset += header.FileHeader.Length;
                    _cdHeaders.Add(header);
                }
            }

            else
            {
                throw new ArgumentException("End of Directory Header could not be found!");
            }
        }

        public async Task ExtractAll(string path, CancellationToken token)
        {
            foreach (var fileHeader in _cdHeaders)
            {
                if (fileHeader.CompressedSize != fileHeader.UncompressedSize) { throw new ArgumentException("Unable to extract, file is compressed!"); }

                _stream.Seek(fileHeader.LocalHeaderOffset + 516, SeekOrigin.Begin);

                var locFileHeaderSpan = new byte[LocalFileHeader.StaticHeaderSize + fileHeader.FilenameLength];
                await _stream.ReadAsync(locFileHeaderSpan, token);

                var locFileHeader = new LocalFileHeader(locFileHeaderSpan);

                var fileData = new byte[locFileHeader.UncompressedSize];
                await _stream.ReadAsync(fileData, token);

                var file = new PkZipFile(locFileHeader, fileHeader.LocalHeaderOffset, fileData);

                var filename = file.Filename;
                var filePath = Path.Combine(path, filename);

                if (!Regex.Match(filename, @"^(?!.*\.$)(?!.*?\.\.)[a-zA-Z0-9_.\\\/-]+$").Success) { throw new ArgumentException($"Invalid Filename: { filename }"); }
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = File.Create(filePath))
                {
                    await stream.WriteAsync(file.File, token);
                }
            }
        }
    }
}
