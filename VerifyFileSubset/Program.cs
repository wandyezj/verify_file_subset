using System;
using System.Collections.Generic;

namespace VerifyFileSubset
{
    // Verify that a json file contains the subset of another json file

    class Program
    {
        private const int ExitCodeInvalidParameters = unchecked((int)0x80070057); //E_INVALIDARG
        private const int ExitCodeVerificationFail = 1; // S_FALSE
        private const int ExitCodeVerificationPass = 0; // S_OK

        private static string help = $@"
Usage:
    [.verify.type file path] [.type file path] 

    available .type are: {{.json, .xml, .reg}}

Example:

    subset.verify.json superset.verify.json
";

        private static string help_detailed = $@"
{help}

# Overview

This program validates that a verify file is a subset of a subject file.

Valid file types: {{.json, .xml, .reg}}



## Json

This program validates that a .json file contains entries specified in a .verify.json file.

The .verify.json file follows the same format as the .json file.



## Xml

This program validates that a .xml file contains a superset of a .verify.xml file.

The .verify.xml file follows the same format as the .xml file.



## Reg

This program validates that a .reg file contains entries specified in a .verify.reg file.

The .verify.reg file follows the same format as the .reg file with some exceptions.

Examples:
    ; Check that a key is present
    [key]

    ; Check that a key is not present
    [-key]

    ; Check that a value is not present
    [key]
    'value'=-

    ; Check that values are present
    [key]
    'valueD'=dword
    'valueB'=hex
    'valueN'=hex(0)
    'valueE'=hex(2)
    'valueM'=hex(7)
    'valueQ'=hex(b)

    ; Check that a value has expected data
    [key]
    'value'=dword:00000002

    ; Check that a string value is present (different format from .reg file)
    [key]
    'valueS'=string

    ; Check that a string value has expected data
    [key]
    'valueS'='string data'



# Return Code:

    {ExitCodeVerificationPass} - [pass] Verification
    {ExitCodeVerificationFail} - [FAIL] Verification
    X - Invalid Input (or help)

# Source

https://github.com/wandyezj/verify_file_subset

".Replace("'", "\"");

        public static void ShowResult(bool pass)
        {
            Console.Write("[");

            Console.ForegroundColor = pass ? Console.ForegroundColor : ConsoleColor.DarkRed;
            Console.Write(pass ? "pass" : "FAIL");
            Console.ResetColor();

            Console.Write("]");
        }

        private static void ActionHelp()
        {
            Console.WriteLine(help);
        }

        private static void ActionDetailedHelp()
        {
            Console.WriteLine(help_detailed);
        }

        private static void ActionInvalid(string argument_issues)
        {
            Console.WriteLine("Invalid Arguments");
            Console.WriteLine(argument_issues);
            ActionHelp();
        }

        public static int ActionVerify(string verifyFilePath, string subjectFilePath)
        {
            bool is_subset = Verify.VerifyFiles(verifyFilePath, subjectFilePath);
            ShowResult(is_subset);
            return is_subset ? ExitCodeVerificationPass : ExitCodeVerificationFail;
        }

        private static Dictionary<VerifyArguments.RunAction, Func<VerifyArguments, int>> ActionMap = new Dictionary<VerifyArguments.RunAction, Func<VerifyArguments, int>>()
        {
            {VerifyArguments.RunAction.Help, (args) => {ActionHelp(); return ExitCodeInvalidParameters; } },
            {VerifyArguments.RunAction.DetailedHelp, (args) => {ActionDetailedHelp(); return ExitCodeInvalidParameters; } },
            {VerifyArguments.RunAction.Invalid, (args) => {ActionInvalid(args.ArgumentIssues); return ExitCodeInvalidParameters; } },
            {VerifyArguments.RunAction.Verify, (args) => {return ActionVerify(args.VerifyFilePath, args.SubjectFilePath);} },
        };

        static int Main(string[] args)
        {
            var arguments = new VerifyArguments(args);

            var result = ActionMap[arguments.Action](arguments);

            return result;
        }
    }
}
