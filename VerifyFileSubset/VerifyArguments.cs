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

        // Restriction on file extensions, enforce .verify.reg to make sure intent is clear
        private static readonly string[] FileAllowableSuffix = { ".json", ".xml", ".reg" };
        private static readonly string[] VerifyFileAllowableSuffix = { ".json", ".xml", ".verify.reg" };

        private static readonly char[] HelpPrefix = { '-', '/', '\\' };
        private static readonly List<string> HelpOptions = new List<string> { "?", "h", "help" };

        public string ArgumentIssues { get; private set; }

        public RunAction Action { get; private set; }

        public string VerifyFilePath { get; private set; }

        public string SubjectFilePath { get; private set; }

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
                    SubjectFilePath = args[1];

                    var errors = new StringBuilder();

                    if (FileExtensionsMatch(VerifyFilePath, SubjectFilePath, ref errors) && FilePathArgumentOk(VerifyFilePath, VerifyFileAllowableSuffix, ref errors) && FilePathArgumentOk(SubjectFilePath, FileAllowableSuffix, ref errors))
                    {
                        Action = RunAction.Verify;
                    }
                    ArgumentIssues = errors.ToString();
                    break;
                default:
                    break;
            }
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


        private static bool FileExtensionsMatch(string verifyFilePath, string subjectFilePath, ref StringBuilder errors)
        {
            var verifyExtension = Path.GetExtension(verifyFilePath).ToLower();
            var subjectEntension = Path.GetExtension(subjectFilePath).ToLower();

            bool extensionsMatch = verifyExtension == subjectEntension;

            if (!extensionsMatch)
            {
                errors.Append($"Verify [{verifyExtension}] and Subject [{subjectEntension}] do not have the same extensions.");
            }

            return extensionsMatch;
        }


    }
}
