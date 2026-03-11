using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    /// Uses UnityEngine.Mathf — runs as Unity EditMode tests.
    /// </summary>
    [TestFixture]
    public class NumberExtensionsTests
    {
        // ── PercentageOf ──────────────────────────────────────────────────────

        [Test]
        public void PercentageOf_ValidValues_ReturnsCorrectFraction()
        {
            Assert.AreEqual(0.5f, 1.PercentageOf(2));
        }

        [Test]
        public void PercentageOf_WholeIsZero_ReturnsZero()
        {
            Assert.AreEqual(0f, 5.PercentageOf(0));
        }

        [Test]
        public void PercentageOf_PartEqualsWhole_Returns1()
        {
            Assert.AreEqual(1f, 3.PercentageOf(3));
        }

        [Test]
        public void PercentageOf_PartGreaterThanWhole_ReturnsGreaterThan1()
        {
            Assert.Greater(4.PercentageOf(2), 1f);
        }

        // ── IsOdd / IsEven ────────────────────────────────────────────────────

        [Test]
        public void IsOdd_OddNumber_ReturnsTrue()
        {
            Assert.IsTrue(3.IsOdd());
        }

        [Test]
        public void IsOdd_EvenNumber_ReturnsFalse()
        {
            Assert.IsFalse(4.IsOdd());
        }

        [Test]
        public void IsEven_EvenNumber_ReturnsTrue()
        {
            Assert.IsTrue(8.IsEven());
        }

        [Test]
        public void IsEven_OddNumber_ReturnsFalse()
        {
            Assert.IsFalse(7.IsEven());
        }

        // ── AtLeast / AtMost (int) ────────────────────────────────────────────

        [Test]
        public void AtLeast_Int_ValueBelowMin_ReturnsMin()
        {
            Assert.AreEqual(5, 2.AtLeast(5));
        }

        [Test]
        public void AtLeast_Int_ValueAboveMin_ReturnsValue()
        {
            Assert.AreEqual(10, 10.AtLeast(3));
        }

        [Test]
        public void AtMost_Int_ValueAboveMax_ReturnsMax()
        {
            Assert.AreEqual(5, 10.AtMost(5));
        }

        [Test]
        public void AtMost_Int_ValueBelowMax_ReturnsValue()
        {
            Assert.AreEqual(3, 3.AtMost(10));
        }

        // ── AtLeast / AtMost (float) ──────────────────────────────────────────

        [Test]
        public void AtLeast_Float_ValueBelowMin_ReturnsMin()
        {
            Assert.AreEqual(2.5f, 1.0f.AtLeast(2.5f));
        }

        [Test]
        public void AtMost_Float_ValueAboveMax_ReturnsMax()
        {
            Assert.AreEqual(3.0f, 9.0f.AtMost(3.0f));
        }

        // ── AtLeast / AtMost (double) ─────────────────────────────────────────

        [Test]
        public void AtLeast_Double_ValueBelowMin_ReturnsMin()
        {
            Assert.AreEqual(5.0, 1.0.AtLeast(5.0));
        }

        [Test]
        public void AtMost_Double_ValueAboveMax_ReturnsMax()
        {
            Assert.AreEqual(4.0, 9.0.AtMost(4.0));
        }
    }
}
