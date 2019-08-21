using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>

    /// </summary>
    class CompressWorker
    {
        /// <summary>
        /// byte array for compression and save
        /// </summary>
        public byte[] partOfFile { get; set; }

        /// <summary>
        /// full path where compresed file will be saved
        /// </summary>
        public string outputFullPath { get; set; }

        public CompressWorker(byte[] partOfFile, string outputFullPath)
        {
            this.partOfFile = partOfFile;
            this.outputFullPath = outputFullPath;
        }

        /// <summary>
        /// compress & save one files 
        /// </summary>
        /// <returns>if operation was successful</returns>
        public async Task<bool> compressTask()
        {
            try
            {
                using (FileStream compressedFileStream = File.Create(outputFullPath))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        compressionStream.Write(partOfFile, 0, partOfFile.Count());
                    }
                }
                Debug.WriteLine($"Compressed {outputFullPath} to {partOfFile.Count()} bytes."); //TODO
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// single thread static file save 
        /// </summary>
        /// <param name="partOfFile">part of the file ib byte[]</param>
        /// <param name="outputFullPath">path fo output file</param>
        public static void simpleCompress(byte[] partOfFile, string outputFullPath)
        {
            using (FileStream compressedFileStream = File.Create(outputFullPath))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    compressionStream.Write(partOfFile, 0, partOfFile.Count());
                }
            }
            Debug.WriteLine($"Compressed {outputFullPath} to {partOfFile.Count()} bytes."); //TODO
        }

        /// <summary>
        /// static single thread, file load & decompress
        /// </summary>
        /// <param name="inputFullPath">path to input file</param>
        /// <returns>loaded & decompressed file</returns>
        public static byte[] simpleDecompress(string inputFullPath)
        {
            byte[] loadedFile;

            using (FileStream originalFileStream = new FileStream(inputFullPath,FileMode.Open))
            {
                byte[] localBufffer = new byte[1];
                using (MemoryStream decompressedStream = new MemoryStream())
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedStream);
                    }
                    loadedFile = decompressedStream.ToArray();
                }
            }
            Debug.WriteLine($"Decompresed file {inputFullPath} to {loadedFile.Length} bytes."); //TODO
            
            return loadedFile;
        }

    }
}
