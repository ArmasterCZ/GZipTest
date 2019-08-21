using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class Program
    {
        /// <summary>
        /// entry point of program
        /// </summary>
        /// <param name="args">arguments in order [compress/decompress] [input file/archive] [output archive/file]</param>
        /// <returns>0 - if process was successful or 1 if was not</returns>
        static int Main(string[] args)
        {
            //TODO
            //args = new string[] { "compress", @"D:\Armaster\Downloads\MoW2 clip setting.MP4", @"D:\Armaster\Downloads\zipTest\zipedFile.gm" };
            //args = new string[] { "compress", @"D:\Armaster\Downloads\mongodb-win32-x86_64-2008plus-ssl-4.0.10-signed.msi", @"D:\Armaster\Downloads\zipTest\zipedFile.gm" };
            //args = new string[] { "compress", @"D:\Armaster\Downloads\randomDocument.txt", @"D:\Armaster\Downloads\zipTest\zipedFile.gm" };
            //args = new string[] { "decompress", @"D:\Armaster\Downloads\zipTest\zipedFile1.gz", @"D:\Armaster\Downloads\zipTest\unziped.msi" };

            ProcessComposer processComposer = new ProcessComposer();
            processComposer.ErrorMessage += displayMessage;
            bool operationSuccesfull = processComposer.process(args);

            if (operationSuccesfull)
            {
                displayMessage(null, $"{args[0]}ion done.");
            }
            return operationSuccesfull ? 0 : 1;
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



