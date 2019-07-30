using System;
using System.IO;

namespace GZipTest
{
    class Program
    {
        /// <summary>
        /// start point of program, check conditions and decide type of operation
        /// </summary>
        /// <param name="args">arguments in order [compress/decompress] [input file/archive] [output archive/file]</param>
        static void Main(string[] args)
        {
            bool successfulOperation = false;
            if (checkConditionsOfArgs(args))
            {
                string typeOfOperation = args[0];
                ArchiveWorker worker;
                switch (typeOfOperation)
                {
                    case "compress":
                        worker = new CompressWorker(args[1], args[2]);
                        break;

                    case "decompress":
                        worker = new DecompressWorker(args[1], args[2]);
                        break;

                    default:
                        worker = new ArchiveWorker(args[1], args[2]);
                        successfulOperation = false;
                        break;
                }
                worker.showMessage += displayMessage;
                successfulOperation = worker.process();
            }

            Console.WriteLine(successfulOperation);
        }

        /// <summary>
        /// check if inserted conditions are in correct format
        /// </summary>
        /// <param name="args">field of conditions</param>
        /// <returns>conditions are correct</returns>
        private static bool checkConditionsOfArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine($"Please enter arguments. {Environment.NewLine}Example: GZipTest.exe compress [original file name] [archive file name] ");
                return false;
            }
            else if (args.Length > 3)
            {
                Console.WriteLine("Not correct number of arguments. Please make sure there are no spaces in entered path.");
                return false;
            }
            else if (args.Length == 3)
            {
                string operation = args[0];
                string inputPath = args[1];
                string outputPath = args[2];

                if (!( operation.Equals("compress") || operation.Equals("decompress")))
                {
                    Console.WriteLine($"First argument need to be compress or decompress");
                    return false;
                }

                if (!File.Exists(inputPath))
                {
                    Console.WriteLine($"Input file not exist: {inputPath}");
                    return false;
                }

                if (File.Exists(outputPath))
                {
                    Console.WriteLine($"Output file aleredy exist. Please delete it first. {inputPath}");
                    return false;
                }

                return true;
            }
            else
            {
                Console.WriteLine("Not correct number of arguments.");
                return false;
            }
        }

        /// <summary>
        /// method for display message from callback
        /// </summary>
        private static void displayMessage(object sender, string message)
        {
            Console.WriteLine(message);
        }

    }
}

/*
 cls
#
C:\Users\Armaster\source\repos\2019.07.29_GZipTest\GZipTest\obj\Debug\GZipTest.exe compress D:\Armaster\Downloads\zipTest\bonus.txt D:\Armaster\Downloads\zipTest\bonusZiped
#C:\Users\Armaster\source\repos\2019.07.29_GZipTest\GZipTest\obj\Debug\GZipTest.exe decompress D:\Armaster\Downloads\zipTest\bonusZiped0.gz D:\Armaster\Downloads\zipTest\bonusOutput.txt

#C:\Users\Armaster\source\repos\2019.07.29_GZipTest\GZipTest\obj\Debug\GZipTest.exe compress D:\Armaster\Downloads\zipTest\input.pdf D:\Armaster\Downloads\zipTest\zipedItem
#C:\Users\Armaster\source\repos\2019.07.29_GZipTest\GZipTest\obj\Debug\GZipTest.exe decompressing D:\Armaster\Downloads\zipTest\zipedItem0.gz D:\Armaster\Downloads\zipTest\inputUnziped.pdf
*/


