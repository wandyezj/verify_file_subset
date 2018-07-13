using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyFileSubset;

namespace UnitTests
{
    [TestClass]
    public class UnitTestJson
    {
        [TestMethod]
        public void TestEmptyObjectEqual()
        {
            var verify = "{}";
            var subject = "{}";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }

        [TestMethod]
        public void TestEmptyArrayEqual()
        {
            var verify = "[]";
            var subject = "[]";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }

        [TestMethod]
        public void TestObjectSubsetEqual()
        {
            var verify = @"{'key':'value'}";
            var subject = @"{'key':'value', 'key2':'value2'}";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }

        [TestMethod]
        public void TestObjectSubsetKeyMissingNotEqual()
        {
            var verify = @"{'missing_key':'value'}";
            var subject = @"{'key':'value', 'key2':'value2'}";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }

        [TestMethod]
        public void TestObjectSubsetKeyValueDifferentNotEqual()
        {
            var verify = @"{'key':'value'}";
            var subject = @"{'key':'different_value', 'key2':'value2'}";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }


        [TestMethod]
        public void TestArraySubsetEqual()
        {
            var verify = @"[1, 3]";
            var subject = @"[1, 2, 3, 4]";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }

        [TestMethod]
        public void TestArraySubsetValueMissingEqual()
        {
            var verify = @"[1, 'missing']";
            var subject = @"[1, 2, 3, 4]";
            var expected = false;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }


        [TestMethod]
        public void TestObjectMultilevelEqual()
        {
            var verify = @"{'key':{'key':'value'}}";
            var subject = @"{'key':{'key':'value'}, 'key_b':'value'}";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }

        [TestMethod]
        public void TestArrayMultilevelEqual()
        {
            var verify = @"[['value'], ['value','value', []]]";
            var subject = @"[['value_other', 'value'], ['value','value', 'value_middle', []], ['value']]";
            var expected = true;

            Assert.AreEqual(expected, Verify.VerifyJsonText(verify, subject));
        }


    }
}
