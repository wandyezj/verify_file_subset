using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyFileSubset;

namespace UnitTests
{
    [TestClass]
    public class XmlUnitTest
    {
        [TestMethod]
        public void TestBasicEqual()
        {
            var verify = "<tag/>";
            var subject = "<tag/>";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestBasicNotEqual()
        {
            var verify = "<tag/>";
            var subject = "<other/>";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestBasicSubsetEqual()
        {
            var verify = "<tag><child></child></tag>";
            var subject = "<tag><other></other><child></child></tag>";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestBasicSubsetNotEqual()
        {
            var verify = "<tag><child></child></tag>";
            var subject = "<tag><other></other></tag>";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestBasicAttributeValuesEqual()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag attribute='value'/>";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestBasicAttributeValuesNotEqual()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag attribute='othervalue'/>";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }


        [TestMethod]
        public void TestBasicAttributeMissing()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag/>";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

        [TestMethod]
        public void TestBasicAttributeDifferent()
        {
            var verify = "<tag attribute='value'/>";
            var subject = "<tag attributeother='value'/>";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyTextXml(verify, subject));
        }

    }
}
