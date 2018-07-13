using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VerifyFileSubset
{
    class VerifyArguments
    {
        public enum RunAction { Invalid, Help, DetailedHelp, Verify };

        // Restriction on file extensions, enforce .verify to make sure intent is clear
        private static readonly string[] FileAllowableSuffix = { ".json" };
        private static readonly string[] VerifyFileAllowableSuffix = { ".verify.json" };

        private static readonly char[] HelpPrefix = { '-', '/', '\\' };
        private static readonly List<string> HelpOptions = new List<string> { "?", "h", "help" };

        public string ArgumentIssues { get; private set; }

        public RunAction Action { get; private set; }

        public string VerifyFilePath { get; private set; }

        public string FilePath { get; private set; }

        public VerifyArguments(string[] args)
        {
            Action = RunAction.Invalid;

            switch (args.Length)
            {
                case 0:
                    Action = RunAction.Help;
                    break;
                case 1:
                    if (HelpOptions.Contains(args[0].TrimStart(HelpPrefix)))
                    {
                        Action = RunAction.DetailedHelp;
                    }
                    break;
                case 2:
                    VerifyFilePath = args[0];
                    FilePath = args[1];
                    Action = RunAction.Verify;
                    break;
                default:
                    break;
            }

            var errors = new StringBuilder();

            if (!(FilePathArgumentOk(VerifyFilePath, VerifyFileAllowableSuffix, ref errors) && FilePathArgumentOk(FilePath, FileAllowableSuffix, ref errors)))
            {
                Action = RunAction.Invalid;
            }
            ArgumentIssues = errors.ToString();
        }

        /// <summary>
        /// Checks a file path argument, make sure that the file exists and has the right extension.
        /// </summary>
        /// <param name="filePath">file path to check</param>
        /// <param name="allowableExtensions">entensions to allow</param>
        /// <param name="erros">keeps track of any issues</param>
        /// <returns>True iff the file exists and has the expected entension</returns>
        private static bool FilePathArgumentOk(string filePath, string[] allowableExtensions, ref StringBuilder errors)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }
            else if (!allowableExtensions.Any(x => filePath.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Append($"File has invalid extension [{filePath}]\n");
                return false;
            }
            else if (!File.Exists(filePath))
            {
                errors.Append($"file path does not exist [{filePath}].\n");
                return false;
            }

            return true;
        }



    }
}
