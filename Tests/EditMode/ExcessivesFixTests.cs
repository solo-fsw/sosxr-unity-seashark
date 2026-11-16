using System.Collections.Generic;
using NUnit.Framework;
using SOSXR.SeaShark.Excessives;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    ///     Regression tests for MathE.ClampWrap and LinqE.Permuation.
    /// </summary>
    [TestFixture]
    public class ExcessivesFixTests
    {
        [Test]
        public void MathE_ClampWrap_Int_WrapsNegativesIntoRange()
        {
            Assert.AreEqual(9, MathE.ClampWrap(-1, 0, 10));
            Assert.AreEqual(0, MathE.ClampWrap(10, 0, 10));
            Assert.AreEqual(3, MathE.ClampWrap(13, 0, 10));
            Assert.AreEqual(14, MathE.ClampWrap(-6, 5, 15));
            Assert.AreEqual(5, MathE.ClampWrap(42, 5, 5));
        }


        [Test]
        public void MathE_ClampWrap_Float_WrapsNegativeAngles()
        {
            Assert.AreEqual(270f, MathE.ClampWrap(-90f, 0f, 360f), 1e-4f);
            Assert.AreEqual(170f, MathE.ClampWrap(-190f, -180f, 180f), 1e-4f);
            Assert.AreEqual(10f, MathE.ClampWrap(370f, 0f, 360f), 1e-4f);
        }


        [Test]
        public void MathE_ClampWrap_Double_WrapsNegativesIntoRange()
        {
            Assert.AreEqual(270d, MathE.ClampWrap(-90d, 0d, 360d), 1e-9d);
            Assert.AreEqual(-170d, MathE.ClampWrap(190d, -180d, 180d), 1e-9d);
        }


        [Test]
        public void LinqE_Permuation_VisitsEveryPair()
        {
            var source = new List<int> { 1, 2, 3 };
            var pairs = new List<(int, int)>();

            source.Permuation((a, b) => pairs.Add((a, b)));

            Assert.AreEqual(9, pairs.Count);
            Assert.Contains((3, 1), pairs);
            Assert.Contains((2, 2), pairs);
        }
    }
}
