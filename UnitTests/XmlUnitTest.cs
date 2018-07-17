using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyFileSubset;

namespace UnitTests
{
    [TestClass]
    public class XmlUnitTest
    {
        private static void Compare(string verify, string subject, bool expected)
        {
            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestXmlBasicVerifyEmpty()
        {
            var verify = "";
            var subject = "<tag/>";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicSubjectEmpty()
        {
            var verify = "<tag/>";
            var subject = "";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicSubjectAndVerifyEmpty()
        {
            var verify = "";
            var subject = "";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicEqual()
        {
            var verify = "<tag/>";
            var subject = "<tag/>";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicNotEqual()
        {
            var verify = "<tag/>";
            var subject = "<other/>";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicSubsetEqual()
        {
            var verify = "<tag><child></child></tag>";
            var subject = "<tag><other></other><child></child></tag>";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicSubsetNotEqual()
        {
            var verify = "<tag><child></child></tag>";
            var subject = "<tag><other></other></tag>";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicAttributeValuesEqual()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag attribute='value'/>";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicAttributeValuesNotEqual()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag attribute='othervalue'/>";
            var expected = false;

            Compare(verify, subject, expected);
        }


        [TestMethod]
        public void TestXmlBasicAttributeMissing()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag/>";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestXmlBasicAttributeDifferent()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag attributeother='value'/>";
            var expected = false;

            Compare(verify, subject, expected);
        }

    }
}
