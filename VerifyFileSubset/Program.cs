using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerifyFileSubset
{
    class Program
    {
        // Verify that a json file contains the subset of another json file

        class Program
        {

            private static string help = $@"
Usage:
        [.verify.json file path] [.json file path] 
";

            private static string help_detailed = $@"
= Overview =

This program validates that a .json file contains entries specified in a .verify.json file.

The .verify.json file follows the same format as the .json file.
";


            private static void ActionHelp()
            {
                Console.WriteLine(help);
            }

            private static void ActionDetailedHelp()
            {
                ActionHelp();
                Console.WriteLine(help_detailed);
            }

            private static void ActionInvalid(string argument_issues)
            {
                Console.WriteLine("Invalid Arguments");
                Console.WriteLine(argument_issues);
                ActionHelp();
            }

            private static int ActionVerify(string verifyFilePath, string subjectFilePath)
            {
                if (JsonVerifier.Verify(verifyFilePath, subjectFilePath))
                {
                    return 0;
                }

                return 1;
            }

            private static Dictionary<VerifyArguments.RunAction, Func<VerifyArguments, int>> ActionMap = new Dictionary<VerifyArguments.RunAction, Func<VerifyArguments, int>>()
        {
            {VerifyArguments.RunAction.Help, (args) => {ActionHelp(); return 0; } },
            {VerifyArguments.RunAction.DetailedHelp, (args) => {ActionDetailedHelp(); return 0; } },
            {VerifyArguments.RunAction.Invalid, (args) => {ActionInvalid(args.ArgumentIssues); return 1; } },
            {VerifyArguments.RunAction.Verify, (args) => {return ActionVerify(args.VerifyFilePath, args.FilePath);} },
        };

            static int Main(string[] args)
            {
                var arguments = new VerifyArguments(args);

                var result = ActionMap[arguments.Action](arguments);

                return result;
            }
        }
    }
}
