using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyFileSubset;

namespace UnitTests
{
    [TestClass]
    public class RegUnitTest
    {
        private static void Compare(string verify, string subject, bool expected)
        {
            verify = verify.Replace("'", "\"");
            subject = subject.Replace("'", "\"");

            Assert.AreEqual(expected, Verify.VerifyTextReg(verify, subject));
        }

        [TestMethod]
        public void TestRegBasicSubjectEmpty()
        {
            var verify = @"Windows Registry Verify
[key]
'value'='data'
 
 ";
            var subject = @"";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicVerifyEmpty()
        {
            var verify = @"";
            var subject = @"Windows Registry Editor Version 5.00
[key]
'value'='data'
 
 ";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicVerifyAndSubjectEmpty()
        {
            var verify = @"";
            var subject = @"";

            var expected = true;

            Compare(verify, subject, expected);
        }



        [TestMethod]
        public void TestRegBasicEqual()
        {
            var verify = @"Windows Registry Verify
[key]
'value'='data'
 
 ";
            var subject = @"Windows Registry Editor Version 5.00
[key]
'value'='data'
 
 ";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicKeyNotEqual()
        {
            var verify = @"Windows Registry Verify
[key]
'value'='data'
 
 ";
            var subject = @"Windows Registry Editor Version 5.00
[key_other]
'value'='data'
 
 ";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicValueNotEqual()
        {
            var verify = @"Windows Registry Verify
[key]
'value'='data'
 
 ";
            var subject = @"Windows Registry Editor Version 5.00
[key]
'othervalue'='data'
 
 ";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicDataNotEqual()
        {
            var verify = @"Windows Registry Verify
[key]
'value'='data'
 
 ";
            var subject = @"Windows Registry Editor Version 5.00
[key]
'value'='otherdata'
 
 ";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicValueNotPresentNotEqual()
        {
            var verify = @"Windows Registry Verify
[key]
'value'=-
 
 ";
            var subject = @"Windows Registry Editor Version 5.00
[key]
'value'='data'
 
 ";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestRegBasicValueNotPresentEqual()
        {
            var verify = @"Windows Registry Verify
[key]
'value'=-
 
 ";
            var subject = @"Windows Registry Editor Version 5.00
[key]
 
 ";
            var expected = true;

            Compare(verify, subject, expected);
        }



    }
}
