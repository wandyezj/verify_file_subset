using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace VerifyJsonFile
{
    class JsonVerifier
    {
        /// <summary>
        /// Is the file a superset of verify?
        /// 
        /// Checks:
        ///     attributes in verify have a corresponding attribute in the subject (order matters).
        ///
        ///     To be a corresponding attribute:
        ///         * attribute must have the same name
        ///         * attribute present on the verify node must be present
        ///         * attribute children must be corresponding
        ///
        /// </summary>
        /// <param name="verifyFilePath">file that contains the subset to check that are present in verify</param>
        /// <param name="filePath">file to check</param>
        /// <returns>True if xml is a superset of verify</returns>
        public static bool Verify(string verifyFilePath, string subjectFilePath)
        {
            var verifyData = File.ReadAllText(verifyFilePath);

            var subjectData = File.ReadAllText(subjectFilePath);

            return VerifyText(verifyData, subjectData);
        }

        public static bool VerifyText(string verifyText, string subjectText)
        {
            var verify = JObject.Parse(verifyText);

            var subject = JObject.Parse(subjectText);

            return ChildrenMatch(verify, subject);
        }

        public static bool ChildrenMatch(JToken verify, JToken subject)
        {
            if (verify.Type != subject.Type)
            {
                // Types must match
                return false;
            }

            switch(verify.Type)
            {
                case JTokenType.Object:
                {
                    // Declaring that name order does not matter for objects
                    bool all_verify_found = true;

                    foreach (var verify_attribute in (JObject)verify)
                    {
                        Console.WriteLine(verify_attribute.Key);

                        bool verify_found = false;
                        foreach (var subject_attribute in (JObject)subject)
                        {
                            if (verify_attribute.Key == subject_attribute.Key)
                            {
                                if (ChildrenMatch(verify_attribute.Value, subject_attribute.Value))
                                {
                                    Console.WriteLine($"Match: [{verify_attribute.Key}]");
                                    verify_found = true;
                                    break;
                                }
                            }
                        }

                        all_verify_found &= verify_found;
                    }
                    return all_verify_found;
                }
                case JTokenType.Array:
                {
                    // Order matters for arrays
                    var verify_array = (JArray)verify;
                    var subject_array = (JArray)subject;

                    if (verify_array.Count == 0)
                    {
                        // Just a test that there is an array in the subject
                        return true;
                    }

                    if (verify_array.Count > subject_array.Count)
                    {
                        return false;
                    }

                    // Check that each verify item appears in the right order in the array
                    int current_index = 0;
                    bool all_verify_found = true;

                    foreach (var verify_attribute in verify_array)
                    {
                        bool verify_found = false;

                        for(; current_index < subject_array.Count && !verify_found; current_index++)
                        {
                            if (ChildrenMatch(verify_attribute, subject_array[current_index]))
                            {
                                verify_found = true;
                            }
                        }

                        all_verify_found &= verify_found;
                    }
                    
                    return all_verify_found;
                }
                default:
                {
                    if (verify.Equals(subject))
                    {
                        Console.WriteLine($"Match: [{verify.ToString()}] [{subject.ToString()}]");
                        return true;
                    }
                    return false;
                }
            }
        }



    }
}
