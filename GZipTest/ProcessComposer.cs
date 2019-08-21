using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>
    /// decides on the order of required operations
    /// </summary>
    class ProcessComposer
    {
        /// <summary>
        /// event for error messages
        /// </summary>
        public EventHandler<string> ErrorMessage;

        /// <summary>
        /// 
        /// </summary>
        public int MaxSizeOfSavedFile { get; set; } = 536870912;

        /// <summary>
        /// validate inputs and start compress or decompress process
        /// </summary>
        /// <returns>operation succesfull</returns>
        public bool process(string[] agrs)
        {
            UserInputValidator userInput = new UserInputValidator(agrs);
            userInput.ErrorMessage = ErrorMessage;
            if (userInput.Validate())
            {
                OperationType typeOfOperation = userInput.GetTypeOfCommand();
                PathMessenger pathMessenger = new PathMessenger(userInput.getInputPath(), userInput.getOutputPath());
                switch (typeOfOperation)
                {
                    case OperationType.compress:
                        return compress(pathMessenger);
                    case OperationType.decompress:
                        return decompress(pathMessenger);
                     default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// save compressed files thrue multiple threads
        /// </summary>
        /// <param name="pathMessenger"></param>
        /// <returns>success of operation</returns>
        private bool compress(PathMessenger pathMessenger)
        {
            //start reading the file
            using (FileLoader fl = new FileLoader(pathMessenger.inputPath))
            {
                List<byte> loadedFilePart;
                fl.MaxListSize = HardwareExplorer.GetSizeOfByteList();
                fl.errorMessage = ErrorMessage;
                Divider divider = new Divider(pathMessenger, MaxSizeOfSavedFile);

                while (fl.canRead == 1)
                {
                    bool successfulLoad = fl.LoadNextPatr();
                    if (successfulLoad)
                    {
                        //load part of the file
                        loadedFilePart = fl.LoadedFilePart;
                        try
                        {
                            //Divide & compressed & Save file
                            divider.Save(loadedFilePart);
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage?.Invoke(this, $"Error during compress file save. {pathMessenger.getOutputFilePath(fl.readCount)}");
                            return false;
                        }
                        Debug.WriteLine($"Readed part: {fl.readCount}");  //TODO
                    }
                    else
                    {
                        ErrorMessage?.Invoke(this, $"Error during file read. Part {fl.readCount}");
                        return false;
                    }
                }
            }

            System.GC.Collect();
            return true;
        }

        /// <summary>
        /// load and decompress files, and it merge them to single file.
        /// </summary>
        /// <param name="pathMessenger">object with stored paths</param>
        /// <returns>success of operation</returns>
        private bool decompress(PathMessenger pathMessenger)
        {
            List<string> compressedFiles = pathMessenger.getInputFilePaths();

            if (compressedFiles.Count() > 0)
            {
                using (FileStream fsWrite = new FileStream(pathMessenger.outputPath, FileMode.CreateNew))
                {
                    Debug.WriteLine($"Found {compressedFiles.Count} compresed files. "); //TODO

                    byte[] readedFile;
                    foreach (string onefilePath in compressedFiles)
                    {
                        byte[] loadedFile = CompressWorker.simpleDecompress(onefilePath);
                        fsWrite.Write(loadedFile, 0, loadedFile.Length);
                    }
                }
                return true;
            }
            else
            {
                ErrorMessage.Invoke(this, "Cannot found any compressed files. Please edit path to source file.");
                return false;
            }



        }

        /// <summary>
        /// save and compres files in single thread
        /// </summary>
        /// <param name="pathMessenger">object with stored paths</param>
        /// <returns>success of operation</returns>
        private bool compressSingleThread(PathMessenger pathMessenger)
        {
            //start reading the file
            using (FileLoader fl = new FileLoader(pathMessenger.inputPath))
            {
                List<byte> loadedFilePart;
                fl.MaxListSize = HardwareExplorer.GetSizeOfByteList();
                fl.errorMessage = ErrorMessage;

                while (fl.canRead == 1)
                {
                    bool successfulLoad = fl.LoadNextPatr();
                    if (successfulLoad)
                    {
                        //load part of the file
                        loadedFilePart = fl.LoadedFilePart;
                        try
                        {
                            //Save compress file
                            CompressWorker.simpleCompress(loadedFilePart.ToArray(), pathMessenger.getOutputFilePath(fl.readCount));
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage?.Invoke(this, $"Error during compress file save. {pathMessenger.getOutputFilePath(fl.readCount)}");
                            return false;
                        }
                        Debug.WriteLine($"Readed part: {fl.readCount}"); //TODO
                    }
                    else
                    {
                        ErrorMessage?.Invoke(this, $"Error during file read. Part {fl.readCount}");
                        return false;
                    }
                }
            }

            return true;
        }
        
    }


}
