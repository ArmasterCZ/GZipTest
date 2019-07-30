using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GZipTest
{
    /// <summary>
    /// class for decompress files and merge them to final file
    /// </summary>
    class DecompressWorker : ArchiveWorker
    {
        /// <summary>
        /// constructor setting paths
        /// </summary>
        /// <param name="inputPath">path to file that should be splitted and compressed</param>
        /// <param name="outputPath">path where will be saved compressed files. Name of file will contain additional number.</param>
        public DecompressWorker(string inputPath, string outputPath) : base(inputPath, outputPath)
        {

        }

        /// <summary>
        /// start decompress process
        /// </summary>
        /// <returns>is successful</returns>
        public override bool process()
        {
            try
            {
                startDecompress();
                return true;
            }
            catch (Exception exc)
            {
                showMessage?.Invoke(this, $"Something is wrong. Operation cannot be finished. { exc.Message}.");
                return false;
            }
        }
        
        /// <summary>
        /// find compressed files, call <see cref="decompress"/> and <see cref="mergeFiles"/>
        /// </summary>
        private void startDecompress()
        {
            //load file/s name
            FileInfo fi = new FileInfo(inputPath);
            string baseFileNameNumber = Path.GetFileNameWithoutExtension(fi.Name);
            string baseFileName = baseFileNameNumber.Substring(0, baseFileNameNumber.Length - 1);
            int fileNumber = Convert.ToInt32(baseFileNameNumber.Substring(baseFileNameNumber.Length - 1));
            string fileName = baseFileName + fileNumber + ".gz";
            string inputFullPath = Path.Combine(Path.GetDirectoryName(fi.FullName), fileName);

            //check if files exist
            List<string> compressedFiles = new List<string>();
            while (File.Exists(inputFullPath))
            {
                compressedFiles.Add(inputFullPath);
                //showMessage?.Invoke(this, $"File added to list: {inputFullPath}");
                fileNumber++;
                fileName = baseFileName + fileNumber + ".gz";
                inputFullPath = Path.Combine(Path.GetDirectoryName(fi.FullName), fileName);
            }

            //unzip Files
            if (compressedFiles.Count > 0)
            {
                List<string> outputFiles = new List<string>();
                foreach (string compressedFilePath in compressedFiles)
                {
                    string partialOutputFilePath = Path.ChangeExtension(compressedFilePath, ".txt");
                    simpleDecompress(compressedFilePath, partialOutputFilePath);
                    outputFiles.Add(partialOutputFilePath);
                }

                mergeFiles(outputFiles, outputPath);
            }
            else
            {
                showMessage?.Invoke(this, $"Cannot found any input files. {inputFullPath}");
            }


        }

        /// <summary>
        /// decompres .gz file to .txt file
        /// </summary>
        /// <param name="partialInputFilePath">ziped file path</param>
        /// <param name="partialOutputFilePath">new unziped file path</param>
        private void simpleDecompress(string partialInputFilePath, string partialOutputFilePath)
        {
            FileInfo fileToDecompress = new FileInfo(partialInputFilePath);
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = partialOutputFilePath;

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        showMessage?.Invoke(this, $"Decompressed: {fileToDecompress.Name}");
                    }
                }
            }

        }

        /// <summary>
        /// take unziped files and merge them to one (delete them)
        /// </summary>
        /// <param name="files">paths of unziped files</param>
        /// <param name="fullOutputPath">path for final merged file</param>
        private void mergeFiles(List<string> files, string fullOutputPath)
        {
            using (FileStream outputFS = File.OpenWrite(fullOutputPath))
            {
                foreach (string file in files)
                {
                    byte[] input = File.ReadAllBytes(file);
                    outputFS.Write(input, 0, input.Count());
                    File.Delete(file);
                }
            }

            showMessage?.Invoke(this, $"{files.Count} files merged to: {fullOutputPath}");
        }

    }
}
