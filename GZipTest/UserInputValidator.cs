using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{

    /// <summary>
    /// validator for user inpur.
    /// correct length & file status.
    /// </summary>
    class UserInputValidator
    {
        /// <summary>
        /// store array of use inputs
        /// </summary>
        private string[] args;

        /// <summary>
        /// event for error messages
        /// </summary>
        public EventHandler<string> ErrorMessage;

        public UserInputValidator(string[] args)
        {
            this.args = args;
        }

        public bool Validate()
        {
            bool validInput = checkArgumentLenght();
            if (validInput)
            {
                validInput = checkArgumentFormat();
            }
            return validInput;
        }

        public OperationType GetTypeOfCommand()
        {
            var successfulConversion = Enum.TryParse(args[0].ToLower(), out OperationType typeOfOperation);

            if (!successfulConversion)
            {
                typeOfOperation = OperationType.error;
            }
            return typeOfOperation;
        }

        /// <summary>
        /// return second path that user insert
        /// </summary>
        /// <returns>path</returns>
        public string getOutputPath()
        {
            return args[2];
        }

        /// <summary>
        /// return first path that user insert
        /// </summary>
        /// <returns>path</returns>
        public string getInputPath()
        {
            return args[1];
        }

        /// <summary>
        /// check if user added correct number of arguments
        /// </summary>
        /// <returns>lenght is correct</returns>
        private bool checkArgumentLenght()
        {
            if (args.Length == 0)
            {
                ErrorMessage?.Invoke(this, $"Please enter arguments. {Environment.NewLine}Example: GZipTest.exe [compress/decompress] [input file/archive] [output archive/file] ");
                return false;
            }
            else if (args.Length > 3)
            {
                ErrorMessage?.Invoke(this, "Not correct number of arguments. Please make sure there are no spaces in entered path.");
                return false;
            }
            else if (args.Length == 3)
            {
                return true;
            }
            else
            {
                ErrorMessage?.Invoke(this, "Not correct number of arguments.");
                return false;
            }
        }

        /// <summary>
        /// check if user require correct operation
        /// & if paths point to existing/not existing files
        /// </summary>
        /// <returns>format is correct</returns>
        private bool checkArgumentFormat()
        {
                string operation = args[0];
                string inputPath = args[1];
                string outputPath = args[2];

                if (!(operation.Equals("compress") || operation.Equals("decompress")))
                {
                    ErrorMessage?.Invoke(this, $"First argument need to be compress or decompress");
                    return false;
                }

                if (!File.Exists(inputPath))
                {
                    ErrorMessage?.Invoke(this, $"Input file not exist: {inputPath}");
                    return false;
                }

                if (File.Exists(outputPath))
                {
                    ErrorMessage?.Invoke(this, $"Output file aleredy exist. Please delete it first. {inputPath}");
                    return false;
                }

                return true;
        }



    }
}
