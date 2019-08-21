using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>
    /// contain required paths
    /// </summary>
    class PathMessenger
    {
        /// <summary>
        /// store first user added path
        /// </summary>
        public string inputPath { get; private set; }

        /// <summary>
        /// store second user added path
        /// </summary>
        public string outputPath { get; private set; }

        /// <summary>
        /// extension for compressed files
        /// </summary>
        private string comprFileExtension = ".gz";

        public PathMessenger(string inputPath, string outputPath)
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        /// <summary>
        /// create path for output file with number. 
        /// Like from "C:/MyFileName.txt" -> "C:/MyFileName1.gz"
        /// </summary>
        /// <param name="numberOfFile">number added to filename</param>
        /// <returns>created path</returns>
        public string getOutputFilePath(int numberOfFile)
        {
            FileInfo fi = new FileInfo(outputPath);
            string outputFileName = Path.GetFileNameWithoutExtension(fi.Name) + numberOfFile + comprFileExtension;
            string outputFullPath = Path.Combine(Path.GetDirectoryName(fi.FullName), outputFileName);
            return outputFullPath;
        }

        /// <summary>
        /// get compressed files in numerical order
        /// </summary>
        /// <returns>list of compressed files</returns>
        public List<string> getInputFilePaths()
        {
            List<string> requestedFiles = new List<string>();

            int counter = 1;
            string searchedPath = inputPath;
            while (File.Exists(searchedPath))
            {
                requestedFiles.Add(searchedPath);
                counter++;
                searchedPath = getInputFilePath(counter);
            }

            return requestedFiles;
        }

        /// <summary>
        /// get one only one compresed file
        /// </summary>
        /// <param name="numberOfFile"></param>
        /// <returns>one path</returns>
        private string getInputFilePath(int numberOfFile)
        {
            FileInfo fi = new FileInfo(inputPath);
            string inputFileNameAlone = Path.GetFileNameWithoutExtension(fi.Name);
            string inputFileNameFull = inputFileNameAlone.Remove(inputFileNameAlone.Length - 1) + numberOfFile + comprFileExtension;
            string inputFullPath = Path.Combine(Path.GetDirectoryName(fi.FullName), inputFileNameFull);
            return inputFullPath;
        }
    }
}
