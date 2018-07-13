using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Verifiers
{
    /// <summary>
    /// Registry types supported.
    /// </summary>
    public enum ValueType
    {
        Invalid,
        String,
        Dword,
        Qword,
        Binary,
        StringExpand,
        StringMulti,
        None,
        Key
    }

    /// <summary>
    /// Actions a .reg file can take.
    /// </summary>
    public enum ValueAction
    {
        Invalid,
        Add,
        Delete
    }

    /// <summary>
    /// Class to represent an entry in a .reg or a .verify.reg file.
    /// </summary>
    internal class RegistryEntry
    {
        public ValueType Type { get; private set; }
        public ValueAction Action { get; private set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public string Data { get; private set; }

        private const string RemoveDelimiter = "-";

        /// <summary>
        /// Create a registry key entry.
        /// 
        /// keyline format:
        /// [key]
        /// 
        /// </summary>
        /// <param name="keyLine">A registry key line from a .reg file</param>
        public RegistryEntry(string keyLine)
        {
            Type = ValueType.Key;

            Action = ParseAction(keyLine);

            char[] removeDelimiter = { '-' };

            Key = ParseKey(keyLine).TrimStart(removeDelimiter);
        }

        /// <summary>
        /// Creates a registry value entry.
        /// 
        /// keyline format:
        /// [key]
        /// 
        /// valueLine formats:
        /// "valueName"=-
        /// "valueName"=type
        /// "valueName"=type:data
        /// "valueName"="string data"
        /// 
        /// </summary>
        /// <param name="keyLine">A complete key line from a .reg file</param>
        /// <param name="valueLine">A complete value line from a .reg file</param>
        public RegistryEntry(string keyLine, string valueLine)
        {
            var key = ParseKey(keyLine);

            if (ParseAction(key) == ValueAction.Delete)
            {
                throw new ArgumentException($"Invalid key [{keyLine}] for registry value.");
            }

            Key = key;

            int indexValueDelimiter = -1;

            // special case for default value
            if (valueLine.StartsWith("@="))
            {
                Value = "@";
                indexValueDelimiter = valueLine.IndexOf("@=");
            }
            else
            {
                // General case

                indexValueDelimiter = valueLine.IndexOf(@"""=", StringComparison.CurrentCulture);

                if (indexValueDelimiter < 0)
                {
                    throw new ArgumentException($"Invalid value [{valueLine}].");
                }

                // Skip the " in front and " and end of the value name
                Value = valueLine.Substring(1, indexValueDelimiter - 1);

            }

            string dataField = valueLine.Substring(indexValueDelimiter + 2, valueLine.Length - indexValueDelimiter - 2);

            if (string.IsNullOrWhiteSpace(dataField))
            {
                throw new ArgumentException($"Invalid data [{valueLine}].");
            }

            // special case for string
            if (dataField.StartsWith("\""))
            {
                Type = ValueType.String;
                Data = dataField.Substring(1, dataField.Length - 2);
            }
            else
            {
                var typeDelimiterIndex = dataField.IndexOf(":", StringComparison.CurrentCulture);

                var type = dataField;

                Action = ParseAction(type);

                if (Action == ValueAction.Add)
                {
                    Data = null;
                    if (typeDelimiterIndex > 0)
                    {
                        type = dataField.Substring(0, typeDelimiterIndex);
                        Data = dataField.Substring(typeDelimiterIndex);
                    }

                    Type = ParseType(type);
                }
            }
        }

        /// <summary>
        /// Parse out the registry key from a key line [key]
        /// </summary>
        /// <param name="keyLine">A line that contains a registry key in the format [key]</param>
        /// <returns>The key without brackets</returns>
        private static string ParseKey(string keyLine)
        {
            char[] keyDelimiters = { '[', ']' };

            return keyLine.Trim().Trim(keyDelimiters);
        }

        /// <summary>
        /// Figure out the action (add or remove) based on a prefix of -
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static ValueAction ParseAction(string action)
        {
            return action.StartsWith(RemoveDelimiter) ? ValueAction.Delete : ValueAction.Add;
        }

        /// <summary>
        /// Find the type of a registry value based on the string in the .reg or .verify.reg file.
        /// </summary>
        /// <param name="type">string registry type found in the file</param>
        /// <returns>A registry value type</returns>
        private static ValueType ParseType(string type)
        {
            ValueType parsedType;

            switch (type)
            {
                case "dword":
                    parsedType = ValueType.Dword;
                    break;

                case "hex(b)":
                    parsedType = ValueType.Qword;
                    break;

                case "hex(2)":
                    parsedType = ValueType.StringExpand;
                    break;

                case "hex(7)":
                    parsedType = ValueType.StringMulti;
                    break;

                case "hex":
                    parsedType = ValueType.Binary;
                    break;

                case "hex(0)":
                    parsedType = ValueType.None;
                    break;

                // string is only for .verify.reg file
                case "string":
                    parsedType = ValueType.String;
                    break;

                default:
                    throw new ArgumentException($"Unknown Type [{type}].");
            }

            return parsedType;
        }

        private const string RegistryFileType = "Windows Registry Editor Version 5.00";
        private const string VerifyRegistryFileType = "Windows Registry Verify";

        /// <summary>
        /// Parse a .reg file into a RegistryEntry list.
        /// </summary>
        /// <param name="filePath">.reg file path</param>
        /// <returns>List of RegistryEntries in the file</returns>
        public static List<RegistryEntry> ParseRegistryFile(string filePath)
        {
            return ParseRegistryEntries(filePath, RegistryFileType);
        }

        /// <summary>
        /// Parse a .verify.reg file into a RegistryEntry list.
        /// </summary>
        /// <param name="filePath">.verify.reg file path</param>
        /// <returns>List of RegistryEntries in the file</returns>
        public static List<RegistryEntry> ParseVerifyRegistryFile(string filePath)
        {
            return ParseRegistryEntries(filePath, VerifyRegistryFileType);
        }

        /// <summary>
        /// Create a list of registry entry objects from a .reg or a .verify.reg file
        /// </summary>
        /// <param name="filePath">.reg or .verify.reg file</param>
        /// <param name="expectedHeader">expected first line of the file</param>
        /// <returns></returns>
        private static List<RegistryEntry> ParseRegistryEntries(string filePath, string expectedHeader)
        {
            var entries = new List<RegistryEntry>();

            var lines = File.ReadAllLines(filePath);

            var header = (lines.First() ?? "").Trim();

            if (!header.Equals(expectedHeader))
            {
                throw new ArgumentException($"Unexpected file type [{header}] in [{filePath}] Expected [{expectedHeader}]");
            }

            string currentKey = "";
            string fullLine = "";

            var exceptions = false;

            foreach (var line in lines.Skip(1))
            {
                try
                {
                    string trimLine = line.Trim();

                    if (line.StartsWith(";"))
                    {
                        // Ignore comment lines.
                    }
                    else if (line.StartsWith("["))
                    {
                        currentKey = trimLine;

                        entries.Add(new RegistryEntry(currentKey));
                        fullLine = "";
                    }
                    else if (!string.IsNullOrWhiteSpace(trimLine) && trimLine.EndsWith("\\"))
                    {
                        char[] multilineDelimiter = { '\\' };

                        fullLine += trimLine.TrimEnd(multilineDelimiter);
                    }
                    else if (!string.IsNullOrWhiteSpace(trimLine))
                    {
                        fullLine += trimLine;
                        entries.Add(new RegistryEntry(currentKey, fullLine));
                        fullLine = "";
                    }
                }
                catch (Exception e)
                {
                    var initialColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(e.Message);

                    Console.ForegroundColor = initialColor;

                    exceptions = true;
                }
            }

            if (exceptions)
            {
                throw new ArgumentException($"Failed to parse: [{header}] [{filePath}]");
            }

            return entries;
        }

    }
}

