using System.IO;
using Verifiers;

namespace VerifyFileSubset
{
    /// <summary>
    /// Class to hook into the shared verify function, used by the program and unit tests.
    /// Allows unit tests direct access to the same call that the program makes
    /// </summary>
    public class Verify
    {
        public static bool VerifyFiles(string verifyFilePath, string subjectFilePath)
        {
            switch(Path.GetExtension(verifyFilePath).ToLower())
            {
                case ".json":
                    return VerifyFilesJson(verifyFilePath, subjectFilePath);
                case ".xml":
                    return VerifyFilesXml(verifyFilePath, subjectFilePath);
                case ".reg":
                    return VerifyFilesReg(verifyFilePath, subjectFilePath);
                default:
                    return false;
            }
        }

        private static bool VerifyFilesJson(string verifyFilePath, string subjectFilePath)
        {
            var verifyData = File.ReadAllText(verifyFilePath);

            var subjectData = File.ReadAllText(subjectFilePath);

            return VerifyJsonText(verifyData, subjectData);
        }

        private static bool VerifyFilesXml(string verifyFilePath, string subjectFilePath)
        {
            return XmlVerifier.Verify(verifyFilePath, subjectFilePath);
        }

        private static bool VerifyFilesReg(string verifyFilePath, string subjectFilePath)
        {
            return RegVerifier.Verify(verifyFilePath, subjectFilePath);
        }

        /// <summary>
        /// Test hook to allow direct access for unit testing also called by main program.
        /// </summary>
        /// <param name="verify">verify json</param>
        /// <param name="subject">subject json</param>
        /// <returns></returns>
        public static bool VerifyJsonText(string verify, string subject)
        {
            return JsonVerifier.VerifyText(verify, subject);
        }

    }
}
