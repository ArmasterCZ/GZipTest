using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>
    /// allow split of byte[] on multiple part based on <see cref="MaxFileSize"/>
    /// </summary>
    class Divider
    {
        /// <summary>
        /// list for slitting
        /// </summary>
        public List<byte> MainFilePart { get; set; }

        /// <summary>
        /// object for transfer file paths
        /// </summary>
        public PathMessenger pathMessenger { get; set; }

        /// <summary>
        /// maximum number of byte[] for saved file
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// cout how many iteration of save was done. used for file name generation.
        /// </summary>
        public int CycleCount { get; private set; } = 0;

        public Divider(PathMessenger pathMessenger, int MaxFileSize)
        {
            this.pathMessenger = pathMessenger;
            this.MaxFileSize = MaxFileSize;
        }

        /// <summary>
        /// decide if need to use multi/single thread for compress and save
        /// </summary>
        /// <param name="MainFilePart">Part of the file for compress and save</param>
        /// <returns></returns>
        public bool Save(List<byte> MainFilePart)
        {
            this.MainFilePart = MainFilePart;
            if (MainFilePart.Count < MaxFileSize )
            {
                CompressWorker.simpleCompress(MainFilePart.ToArray(), pathMessenger.getOutputFilePath(1));
                Debug.WriteLine($"Compressed file saved: {pathMessenger.getOutputFilePath(1)}");
                return true; //TODO simpleCompress need return
            }
            else
            {
                return ParalelSave();
            }
        }

        /// <summary>
        /// split <see cref="MainFilePart"/> to multiple small ones.
        /// then parallel compress and save them.
        /// </summary>
        /// <returns>success of operation</returns>
        private bool ParalelSave()
        {
            List<byte> currentListPart;
            int mainFilePartCount = MainFilePart.Count();
            int currentStartIndex = 0;
            int currentrequiredSize = MaxFileSize;

            //calculate number of parst. That orginal list will be splited.
            int numberOfPart = (int)(Math.Ceiling((double)mainFilePartCount / (double)MaxFileSize));
            Task<bool>[] compressTasks = new Task<bool>[numberOfPart];

            for (int Count = 0; Count < numberOfPart; Count++)
            {
                CycleCount++;
                //start position for reading in list
                currentStartIndex = (Count * MaxFileSize);
                //requested size.
                currentrequiredSize = ((currentStartIndex + MaxFileSize) > mainFilePartCount) ? (mainFilePartCount - currentStartIndex) : MaxFileSize;
                //get part of the list
                currentListPart = MainFilePart.GetRange(currentStartIndex, currentrequiredSize);

                CompressWorker compressWorker = new CompressWorker(currentListPart.ToArray(), pathMessenger.getOutputFilePath(CycleCount));
                compressTasks[Count] = Task<bool>.Factory.StartNew((item) =>
                {
                   return compressWorker.compressTask().Result;
                }, compressWorker);
            }

            //check if all Task are succesfully finished
            if (compressTasks.Where(x => x.Result == false).ToArray().Count() > 0)
            {
                return false;
            }
            else
            {
                Debug.WriteLine($"Compressed files saved. Files: {compressTasks.Length}");
                return true;
            }

        }

    }
}
