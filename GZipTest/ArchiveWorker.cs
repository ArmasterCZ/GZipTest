using System;
using System.Diagnostics;


namespace GZipTest
{
    /// <summary>
    /// base class for <see cref="CompressWorker"/> and <see cref="DecompressWorker"/>
    /// </summary>
    class ArchiveWorker
    {
        /// <summary>
        /// user inserted path of source file
        /// </summary>
        public string inputPath { get; set; }

        /// <summary>
        /// user inserted path of output file
        /// </summary>
        public string outputPath { get; set; }

        /// <summary>
        /// max size of one chunks of file
        /// </summary>
        public double maxFileSizeKb { get; set; } = 2.024;

        /// <summary>
        /// event for send messages about progress
        /// </summary>
        public EventHandler<string> showMessage;

        public ArchiveWorker(string inputPath, string outputPath)
        {
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        /// <summary>
        /// method for start process of compression/decompression
        /// </summary>
        /// <returns>process was successful</returns>
        public virtual bool process()
        {
            return false;
        }

        /// <summary>
        /// return available RAM
        /// </summary>
        protected int getAvailableMemoryKb()
        {
            int avalebleMemoryKb = 0;
            try
            {
                using (PerformanceCounter ram = new PerformanceCounter("Memory", "Available MBytes", null))
                {
                    avalebleMemoryKb = Convert.ToInt32(ram.NextValue()) * 1000;
                    //showMessage?.Invoke(this, "Available Memory: " + avalebleMemory);
                }
            }
            catch (Exception)
            {
                avalebleMemoryKb = 100 * 1000;
                showMessage?.Invoke(this, "Cannot get available memory. Memmory was set to " + avalebleMemoryKb);
            }
            return avalebleMemoryKb;

        }

        /// <summary>
        /// calculate how many files can be processed at once
        /// </summary>
        /// <returns>number of processes</returns>
        protected int calculateNuberOfThreads()
        {
            int numberOfThreads = 1;
            int availableMemoryKb = getAvailableMemoryKb();
            double maxFileSizeKb = this.maxFileSizeKb;

            if (availableMemoryKb > maxFileSizeKb)
            {
                numberOfThreads = availableMemoryKb / (int)maxFileSizeKb;
            }

            return numberOfThreads;
        }

    }
}
