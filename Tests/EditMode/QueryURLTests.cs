using NUnit.Framework;
using UnityEngine.TestTools;

namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class QueryURLTests
    {
        // Simple test object with public fields and properties
        private class SampleData
        {
            public string Name = "Alice";
            public int Age = 30;
            public string Country = "Netherlands";

            public float Score { get; } = 9.5f;
        }

        // Object with Unity-style private backing fields
        private class PrivateFieldData
        {
#pragma warning disable CS0414 // Field assigned but never used - intentional for testing field filtering
            private string m_username = "bob";
#pragma warning restore CS0414
            public string PublicName = "Bob";
        }

        private const string BaseURL = "https://api.example.com/query";

        [Test]
        public void BuildQueryURL_NullBaseURL_ReturnsEmpty()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, "BaseURL is null or empty.");
            SampleData data = new();
            string result = data.BuildQueryURL(null, "Name");
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void BuildQueryURL_WhitespaceBaseURL_ReturnsEmpty()
        {
            LogAssert.Expect(UnityEngine.LogType.Error, "BaseURL is null or empty.");
            SampleData data = new();
            string result = data.BuildQueryURL("   ", "Name");
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void BuildQueryURL_NoParamNames_ReturnsBaseURL()
        {
            SampleData data = new();
            string result = data.BuildQueryURL(BaseURL);
            Assert.AreEqual(BaseURL, result);
        }

        [Test]
        public void BuildQueryURL_SinglePublicField_AppendsToURL()
        {
            SampleData data = new();
            string result = data.BuildQueryURL(BaseURL, "Name");
            Assert.AreEqual($"{BaseURL}?Name=Alice", result);
        }

        [Test]
        public void BuildQueryURL_MultipleFields_AppendsAll()
        {
            SampleData data = new();
            string result = data.BuildQueryURL(BaseURL, "Name", "Age");
            StringAssert.Contains("Name=Alice", result);
            StringAssert.Contains("Age=30", result);
            StringAssert.StartsWith(BaseURL + "?", result);
        }

        [Test]
        public void BuildQueryURL_PublicProperty_Included()
        {
            SampleData data = new();
            string result = data.BuildQueryURL(BaseURL, "Score");
            StringAssert.Contains("Score=", result);
        }

        [Test]
        public void BuildQueryURL_MissingParam_OmittedFromResult()
        {
            SampleData data = new();
            string result = data.BuildQueryURL(BaseURL, "NonExistent");
            // No query param appended; returns base URL as no valid params were found
            Assert.AreEqual(BaseURL, result);
        }

        [Test]
        public void BuildQueryURL_SpecialCharactersInValue_AreURIEncoded()
        {
            var obj = new { Value = "hello world & more" };
            string result = obj.BuildQueryURL(BaseURL, "Value");
            StringAssert.Contains("hello%20world", result);
        }

        [Test]
        public void BuildQueryURL_NormalizesUnityPrivateFieldName()
        {
            PrivateFieldData data = new();
            // m_username normalized to "username" during lookup
            string result = data.BuildQueryURL(BaseURL, "username");
            StringAssert.Contains("username=bob", result);
        }
    }
}
