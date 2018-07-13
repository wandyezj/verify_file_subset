using System;
using System.Collections.Generic;
using System.Text;

namespace Verifiers
{
    public interface IVerifier
    {
        bool VerifyText(string verifyText, string subjectText);
    }
}
