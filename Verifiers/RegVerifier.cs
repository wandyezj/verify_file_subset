using System;
using System.Collections.Generic;
using System.Linq;

namespace Verifiers
{
    /// <summary>
    /// Class to verify a .reg file with a .verify.reg file.
    /// </summary>
    class RegVerifier
    {
        /// <summary>
        /// Verify a .reg file using a .verify.reg file
        /// </summary>
        /// <param name="verifyFilePath">.verify.reg file path</param>
        /// <param name="regFilePath">.reg file path</param>
        /// <returns>true iff the .reg file contains what is expected by the verify.reg file</returns>
        public static bool Verify(string verifyFilePath, string regFilePath)
        {
            List<RegistryEntry> registryEntries = RegistryEntry.ParseRegistryFile(regFilePath);
            List<RegistryEntry> verifyEntries = RegistryEntry.ParseVerifyRegistryFile(verifyFilePath);

            return IsExpected(verifyEntries, registryEntries);
        }

        /// <summary>
        /// Printing function to show the result of a test
        /// </summary>
        /// <param name="pass">test result</param>
        /// <param name="description">description of the test</param>
        /// <param name="expected">expected test result</param>
        /// <param name="actual">actual test result</param>
        /// <returns>test result</returns>
        private static bool ShowStatus(bool pass, string description, string expected = null, string actual = null)
        {
            string compare = (string.IsNullOrWhiteSpace(expected) || string.IsNullOrWhiteSpace(actual))
                ? ""
                : $" [{expected}] [{actual}]";

            ShowResult(pass);
            Console.WriteLine($" {description}{compare}");
            return pass;
        }

        public static void ShowResult(bool pass)
        {
            Console.Write("[");

            Console.ForegroundColor = pass ? Console.ForegroundColor : ConsoleColor.DarkRed;
            Console.Write(pass ? "pass" : "FAIL");
            Console.ResetColor();

            Console.Write("]");
        }

        /// <summary>
        /// Verify expected registry entires match with actual entries and write tests to console.
        /// </summary>
        /// <param name="expected">expected registry entries</param>
        /// <param name="actual">actual registry entries</param>
        /// <returns></returns>
        private static bool IsExpected(List<RegistryEntry> expected, List<RegistryEntry> actual)
        {
            bool isAsExpected = true;

            Console.WriteLine("\nChecking Keys:\n");
            isAsExpected &= VerifyExpectedKeys(expected, actual);

            Console.WriteLine("\n\nChecking Values:\n");
            isAsExpected &= VerifyExpectedValues(expected, actual);

            return isAsExpected;
        }

        /// <summary>
        /// Verify that actual entries contain expected registry values
        /// </summary>
        /// <param name="expected">expected registry values</param>
        /// <param name="actual">actual registry valued to check</param>
        /// <returns>true iff actual entries contain all expected values</returns>
        private static bool VerifyExpectedValues(List<RegistryEntry> expected, List<RegistryEntry> actual)
        {
            bool isAsExpected = true;
            var expectedValues = expected.FindAll(x => x.Type != ValueType.Key);
            var uniqueKeys = expectedValues.Select(x => x.Key).Distinct();

            // Go through by key for easier reading.

            foreach (var key in uniqueKeys)
            {
                var keyValues = expectedValues.FindAll(x => x.Key == key);

                Console.WriteLine($"key [{key}]");

                foreach (var keyValue in keyValues)
                {
                    var value = keyValue.Value;
                    var type = keyValue.Type;
                    var data = keyValue.Data;
                    var action = keyValue.Action;
                    var matchedValues = actual.FindAll(x => (x.Key == key && x.Value == value));

                    // There should zero or one matched value.

                    if (matchedValues.Count > 1)
                    {
                        isAsExpected &= ShowStatus(false, $"value [{value}] has more than one match");
                        continue;
                    }

                    // Validate expected value is present.

                    switch (action)
                    {
                        case ValueAction.Add:
                            isAsExpected &= ShowStatus(matchedValues.Count == 1, $"value [{value}] exists", "1",
                                matchedValues.Count.ToString());
                            break;
                        case ValueAction.Delete:
                            isAsExpected &= ShowStatus(!matchedValues.Any(), $"value [{value}] exists", "0",
                                matchedValues.Count.ToString());
                            break;
                        default:
                            Console.WriteLine($"Unexpected action for value [{value}]");
                            break;
                    }

                    // Validate added values data.

                    if (action == ValueAction.Add && matchedValues.Any())
                    {
                        var matchedValue = matchedValues.First();

                        // Validate added value is the correct type.
                        bool typesMatch = (type == matchedValue.Type);
                        isAsExpected &= ShowStatus(typesMatch, $"value [{value}] type matches", type.ToString(),
                            matchedValue.Type.ToString());

                        // Validate expected data is present.
                        if (typesMatch && data != null)
                        {
                            isAsExpected &= ShowStatus((data == matchedValue.Data), $"value [{value}] data matches");
                        }
                    }
                }

                Console.WriteLine();
            }
            return isAsExpected;
        }

        /// <summary>
        /// verify that actual contains what is expected for registry keys
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns>true iff actual contains expected keys</returns>
        private static bool VerifyExpectedKeys(List<RegistryEntry> expected, List<RegistryEntry> actual)
        {
            bool isAsExpected = true;
            var expectedKeys = expected.FindAll(x => x.Type == ValueType.Key);

            foreach (var expectedKey in expectedKeys)
            {
                var key = expectedKey.Key;
                var action = expectedKey.Action;

                var matchedKeys = actual.FindAll(x => x.Key == key);

                string info = matchedKeys.Any() ? "found" : "missing";

                switch (action)
                {
                    case ValueAction.Add:
                        isAsExpected &= ShowStatus(matchedKeys.Any(), $"key [{key}]", "found", info);
                        break;
                    case ValueAction.Delete:
                        isAsExpected &= ShowStatus(!matchedKeys.Any(), $"key [{key}]", "missing", info);
                        break;
                    default:
                        Console.WriteLine($"Unexpected action for key [{key}]");
                        break;
                }
            }

            return isAsExpected;
        }
    }
}
