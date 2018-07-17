using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyFileSubset;

namespace UnitTests
{
    [TestClass]
    public class JsonUnitTest
    {
        private static void Compare(string verify, string subject, bool expected)
        {
            Assert.AreEqual(expected, Verify.VerifyTextJson(verify, subject));
        }

        [TestMethod]
        public void TestJsonVerifyEmpty()
        {
            var verify = "";
            var subject = "{}";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonSubjectEmpty()
        {
            var verify = "{}";
            var subject = "";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonVerifyAndSubjectEmpty()
        {
            var verify = "";
            var subject = "";
            var expected = true;

            Compare(verify, subject, expected);
        }


        [TestMethod]
        public void TestJsonEmptyObjectEqual()
        {
            var verify = "{}";
            var subject = "{}";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonEmptyArrayEqual()
        {
            var verify = "[]";
            var subject = "[]";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestObjectSubsetEqual()
        {
            var verify = @"{'key':'value'}";
            var subject = @"{'key':'value', 'key2':'value2'}";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonObjectSubsetKeyMissingNotEqual()
        {
            var verify = @"{'missing_key':'value'}";
            var subject = @"{'key':'value', 'key2':'value2'}";
            var expected = false;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonObjectSubsetKeyValueDifferentNotEqual()
        {
            var verify = @"{'key':'value'}";
            var subject = @"{'key':'different_value', 'key2':'value2'}";
            var expected = false;

            Compare(verify, subject, expected);
        }


        [TestMethod]
        public void TestJsonArraySubsetEqual()
        {
            var verify = @"[1, 3]";
            var subject = @"[1, 2, 3, 4]";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonArraySubsetValueMissingEqual()
        {
            var verify = @"[1, 'missing']";
            var subject = @"[1, 2, 3, 4]";
            var expected = false;

            Compare(verify, subject, expected);
        }


        [TestMethod]
        public void TestJsonObjectMultilevelEqual()
        {
            var verify = @"{'key':{'key':'value'}}";
            var subject = @"{'key':{'key':'value'}, 'key_b':'value'}";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonArrayMultilevelEqual()
        {
            var verify = @"[['value'], ['value','value', []]]";
            var subject = @"[['value_other', 'value'], ['value','value', 'value_middle', []], ['value']]";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonObjectIgnoreOrderEqual()
        {
            var verify = @"{'key':'value','key_other':'value_other'}";
            var subject = @"{'key_other':'value_other', 'key':'value'}";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonComplexObjectIgnoreOrderEqual()
        {
            var verify = @"{'key':{'key':'value'},'key_other':[0,0]}";
            var subject = @"{'key_other':[0,0], 'key':{'key':'value'}}";
            var expected = true;

            Compare(verify, subject, expected);
        }

        [TestMethod]
        public void TestJsonValueEqual()
        {
            var verify = @"[0, '0', 0.0, {}]";
            var subject = @"[0, '0', 0.0, {}]";
            var expected = true;

            Compare(verify, subject, expected);
        }
    }
}
