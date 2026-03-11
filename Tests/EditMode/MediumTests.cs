using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class MediumTests
    {
        [Test]
        public void Constructor_SetsChannelCorrectly()
        {
            var medium = new Medium("testChannel");
            Assert.AreEqual("testChannel", medium.Channel);
        }

        [Test]
        public void Constructor_NullData_TypeNameIsUnknown()
        {
            var medium = new Medium("ch");
            Assert.AreEqual("Type Unknown", medium.GetTypeName());
        }

        [Test]
        public void Constructor_NullData_DataStringIsUnknown()
        {
            var medium = new Medium("ch");
            Assert.AreEqual("Data Unknown", medium.GetDataString());
        }

        [Test]
        public void Data_SetToString_TypeNameReflectsStringType()
        {
            var medium = new Medium("ch", "hello");
            Assert.AreEqual(typeof(string).FullName, medium.GetTypeName());
        }

        [Test]
        public void Data_SetToInt_TypeNameReflectsIntType()
        {
            var medium = new Medium("ch", 42);
            Assert.AreEqual(typeof(int).FullName, medium.GetTypeName());
        }

        [Test]
        public void Data_SetToString_DataStringReturnsToString()
        {
            var medium = new Medium("ch", "world");
            Assert.AreEqual("world", medium.GetDataString());
        }

        [Test]
        public void Data_SetToInt_DataStringReturnsIntString()
        {
            var medium = new Medium("ch", 99);
            Assert.AreEqual("99", medium.GetDataString());
        }

        [Test]
        public void Data_Reassigned_TypeAndDataStringUpdated()
        {
            var medium = new Medium("ch", "first");
            medium.Data = 123;

            Assert.AreEqual(typeof(int).FullName, medium.GetTypeName());
            Assert.AreEqual("123", medium.GetDataString());
        }

        [Test]
        public void Data_SetToSameValue_TypeNamePreserved()
        {
            var medium = new Medium("ch", "same");
            medium.Data = "same"; // same reference value — setter should return early
            Assert.AreEqual(typeof(string).FullName, medium.GetTypeName());
        }
    }
}
