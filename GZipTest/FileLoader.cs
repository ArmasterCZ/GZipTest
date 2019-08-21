using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>
    /// Disposable filestream that allow load part of the file to List<byte> based on <see cref="MaxListSize"/>
    /// </summary>
    class FileLoader : IDisposable
    {
        #region prop
        /// <summary>
        /// file path ready for read
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// max size of list for read bytes <see cref="LoadedFilePart"/> 
        /// </summary>
        public long MaxListSize { get; set; }

        /// <summary>
        /// check if file contain more bytes for read. (updated localy)
        /// 0-no/1-yes
        /// </summary>
        public int canRead { get; private set; }

        /// <summary>
        /// number of read cycles. Also the number of calls of <see cref="LoadNextPatr"/>
        /// </summary>
        public int readCount { get; private set; }

        /// <summary>
        /// called when error occur
        /// </summary>
        public EventHandler<string> errorMessage;

        /// <summary>
        /// readed part of file. filled after <see cref="LoadNextPatr"/>
        /// </summary>
        public List<byte> LoadedFilePart { get; private set; }

        private FileStream fileStream;

        #endregion prop

        /// <summary>
        /// ctor. open file.
        /// </summary>
        /// <param name="filePath"></param>
        public FileLoader(string filePath)
        {
            this.FilePath = filePath;
            fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            canRead = fileStream.CanRead ? 1 : 0;
        }

        /// <summary>
        /// load part of the file to <see cref="LoadedFilePart"/>
        /// </summary>
        /// <returns>read was succesfull</returns>
        public bool LoadNextPatr()
        {
            try
            {
                readCount++;
                byte[] localSmallBuffer = new byte[1];
                LoadedFilePart = new List<byte>();

                for (int i = 0; i < MaxListSize; i++)
                {
                    canRead = fileStream.Read(localSmallBuffer, 0, localSmallBuffer.Length);
                    if (canRead == 0)
                        break;
                    LoadedFilePart.Add(localSmallBuffer[0]);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage?.Invoke(this, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// close FileStream.
        /// </summary>
        public void Dispose()
        {
            fileStream.Dispose();
        }
    }
}
