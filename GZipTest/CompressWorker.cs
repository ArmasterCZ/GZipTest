using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GZipTest
{
    /// <summary>
    /// class for split file to smaller chunks and compress them
    /// </summary>
    class CompressWorker : ArchiveWorker
    {
        /// <summary>
        /// constructor setting paths
        /// </summary>
        /// <param name="inputPath">path to file that should be splitted and compressed</param>
        /// <param name="outputPath">path where will be saved compressed files. Name of file will contain additional number.</param>
        public CompressWorker(string inputPath, string outputPath) : base(inputPath, outputPath)
        {

        }

        /// <summary>
        /// start compress process
        /// </summary>
        /// <returns>is successful</returns>
        public override bool process()
        {
            try
            {
                startCompress();
                return true;
            }
            catch (Exception exc)
            {
                showMessage?.Invoke(this, $"Something is wrong. Operation cannot be finished. { exc.Message}.");
                return false;
            }
        }

        /// <summary>
        /// start reading file in batches and call <see cref="simpleCompress"/>
        /// </summary>
        private void startCompress()
        {
            FileInfo fi = new FileInfo(outputPath);
            using (Stream source = File.OpenRead(inputPath))
            {
                byte[] buffer = new byte[((int)maxFileSizeKb * 1000)];
                int bytesRead;
                int readCounter = 0;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (source.CanRead)
                    {
                        buffer = removeGarbageFromByte(buffer);
                    }

                    //Console.WriteLine(Encoding.UTF8.GetString(buffer));

                    //create file
                    string outputFileName = Path.GetFileNameWithoutExtension(fi.Name) + readCounter + ".gz";
                    string outputFullPath = Path.Combine(Path.GetDirectoryName(fi.FullName), outputFileName);
                    simpleCompress(buffer, outputFullPath);
                    readCounter++;
                    buffer = new byte[((int)maxFileSizeKb * 1000)];
                }
            }
        }

        /// <summary>
        /// compress bytes and save it
        /// </summary>
        /// <param name="partOfFile">bytes for save</param>
        /// <param name="outputFullPath">path for new file</param>
        private void simpleCompress(byte[] partOfFile, string outputFullPath)
        {
            //originalFileStream
            using (FileStream compressedFileStream = File.Create(outputFullPath))
            {
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    compressionStream.Write(partOfFile, 0, partOfFile.Count());

                }
            }
            showMessage?.Invoke(this, $"Compressed {outputFullPath} from {inputPath} to {partOfFile.Count().ToString()} bytes.");

        }

        /// <summary>
        /// workaround for remove empty bytes from buffer
        /// </summary>
        /// <param name="bytes">orginal buffer</param>
        /// <returns>buffer without empty fields</returns>
        private byte[] removeGarbageFromByte(byte[] bytes)
        {
            List<byte> listBytes = new List<byte>();

            //TODO: this condition
            if (bytes[bytes.Length - 1] == 0 && bytes[bytes.Length - 2] == 0 && bytes[bytes.Length - 3] == 0)
            {
                listBytes = bytes.Reverse().ToList();

                while (listBytes[0] == 0)
                {
                    listBytes.RemoveAt(0);
                }

                listBytes.Reverse();
                return listBytes.ToArray();
            }
            else
            {
                return bytes;
            }
        }
    }
}
