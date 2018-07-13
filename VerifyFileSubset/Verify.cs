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
            var verifyText = File.ReadAllText(verifyFilePath);

            var subjectText = File.ReadAllText(subjectFilePath);

            switch (Path.GetExtension(verifyFilePath).ToLower())
            {
                case ".json":
                    return VerifyTextJson(verifyText, subjectText);
                case ".xml":
                    return VerifyTextXml(verifyText, subjectText);
                case ".reg":
                    return VerifyTextReg(verifyText, subjectText);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Test hook to allow direct access for unit testing also called by main program.
        /// </summary>
        /// <param name="verify">verify text</param>
        /// <param name="subject">subject text</param>
        /// <returns></returns>
        public static bool VerifyTextJson(string verify, string subject)
        {
            return (new JsonVerifier()).VerifyText(verify, subject);
        }

        /// <summary>
        /// Test hook to allow direct access for unit testing also called by main program.
        /// </summary>
        /// <param name="verify">verify text</param>
        /// <param name="subject">subject text</param>
        /// <returns></returns>
        public static bool VerifyTextXml(string verify, string subject)
        {
            return (new XmlVerifier()).VerifyText(verify, subject);
        }

        /// <summary>
        /// Test hook to allow direct access for unit testing also called by main program.
        /// </summary>
        /// <param name="verify">verify text</param>
        /// <param name="subject">subject text</param>
        /// <returns></returns>
        public static bool VerifyTextReg(string verify, string subject)
        {
            return (new RegVerifier()).VerifyText(verify, subject);
        }
    }
}
