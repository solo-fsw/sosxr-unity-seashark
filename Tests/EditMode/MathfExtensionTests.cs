using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class MathfExtensionTests
    {
        // ── Min (double) ──────────────────────────────────────────────────────

        [Test]
        public void Min_TwoDoubles_ReturnsSmaller()
        {
            Assert.AreEqual(2.0, MathfExtension.Min(2.0, 5.0));
        }

        [Test]
        public void Min_FirstIsSmaller_ReturnsFirst()
        {
            Assert.AreEqual(1.5, MathfExtension.Min(1.5, 3.0));
        }

        [Test]
        public void Min_EqualValues_ReturnsValue()
        {
            Assert.AreEqual(4.0, MathfExtension.Min(4.0, 4.0));
        }

        [Test]
        public void Min_EmptyArray_ReturnsZero()
        {
            Assert.AreEqual(0.0, MathfExtension.Min());
        }

        [Test]
        public void Min_MultipleDoubles_ReturnsSmallest()
        {
            Assert.AreEqual(-10.0, MathfExtension.Min(3.0, -10.0, 0.0, 7.5));
        }

        [Test]
        public void Min_SingleElement_ReturnsThatElement()
        {
            Assert.AreEqual(42.0, MathfExtension.Min(42.0));
        }

        // ── Max (double) ──────────────────────────────────────────────────────

        [Test]
        public void Max_TwoDoubles_ReturnsLarger()
        {
            Assert.AreEqual(5.0, MathfExtension.Max(2.0, 5.0));
        }

        [Test]
        public void Max_FirstIsLarger_ReturnsFirst()
        {
            Assert.AreEqual(9.9, MathfExtension.Max(9.9, 1.1));
        }

        [Test]
        public void Max_EqualValues_ReturnsValue()
        {
            Assert.AreEqual(3.0, MathfExtension.Max(3.0, 3.0));
        }

        [Test]
        public void Max_EmptyArray_ReturnsZero()
        {
            Assert.AreEqual(0.0, MathfExtension.Max());
        }

        [Test]
        public void Max_MultipleDoubles_ReturnsLargest()
        {
            Assert.AreEqual(100.0, MathfExtension.Max(3.0, -10.0, 100.0, 7.5));
        }

        [Test]
        public void Max_NegativeValues_ReturnsLeastNegative()
        {
            Assert.AreEqual(-1.0, MathfExtension.Max(-5.0, -1.0, -10.0));
        }
    }
}
