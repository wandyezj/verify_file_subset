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
        public static bool VerifyJsonFiles(string verifyFilePath, string subjectFilePath)
        {
            var verifyData = File.ReadAllText(verifyFilePath);

            var subjectData = File.ReadAllText(subjectFilePath);

            return VerifyJsonText(verifyData, subjectData);
        }

        public static bool VerifyJsonText(string verify, string subject)
        {
            return JsonVerifier.VerifyText(verify, subject);
        }

    }
}
