using NUnit.Framework;
using SOSXR.SeaShark.Excessives;
using UnityEngine;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    ///     Regression tests for bugs found in the 2026-11-09 code review.
    /// </summary>
    [TestFixture]
    public class RegressionTests
    {
        [Test]
        public void MathE_Clamp_UsesValueMinMaxOrder()
        {
            Assert.AreEqual(0, MathE.Clamp(-5, 0, 10));
            Assert.AreEqual(10, MathE.Clamp(15, 0, 10));
            Assert.AreEqual(5, MathE.Clamp(5, 0, 10));
        }


        [Test]
        public void MathE_ReMap_MapsLinearly()
        {
            Assert.AreEqual(50f, MathE.ReMap(5f, 0f, 10f, 0f, 100f), 1e-4f);
            Assert.AreEqual(50d, MathE.ReMap(5d, 0d, 10d, 0d, 100d), 1e-9d);
        }


        [Test]
        public void MathE_ReMapClamped_ClampsOutput()
        {
            Assert.AreEqual(100f, MathE.ReMapClamped(20f, 0f, 10f, 0f, 100f), 1e-4f);
        }


        [Test]
        public void MathE_PhysicalConstants_HaveRealMagnitudes()
        {
            Assert.AreEqual(1.61622938e-35, MathE.PLANCKLENGTH, 1e-40);
            Assert.AreEqual(6.6740831e-11, MathE.GRAVITATIONALCONSTANT, 1e-16);
        }


        [Test]
        public void ToVector3Int_UsesZComponent()
        {
            Assert.AreEqual(new Vector3Int(1, 2, 3), new Vector3(1.2f, 2.5f, 3.9f).ToVector3Int());
        }


        [Test]
        public void IsNull_DestroyedUnityObject_ReturnsTrue()
        {
            var go = new GameObject("IsNullTest");
            Object.DestroyImmediate(go);

            Assert.IsTrue(go.IsNull());
        }


        [Test]
        public void FindAncestorByTag_ReturnsTaggedAncestor()
        {
            var root = new GameObject("Root") { tag = "MainCamera" };
            var middle = new GameObject("Middle");
            var leaf = new GameObject("Leaf");
            middle.transform.SetParent(root.transform);
            leaf.transform.SetParent(middle.transform);

            try
            {
                Assert.AreSame(root, leaf.FindAncestorByTag("MainCamera"));
                Assert.IsNull(root.FindAncestorByTag("MainCamera"));
            }
            finally
            {
                Object.DestroyImmediate(root);
            }
        }


        [Test]
        public void DestroyChildrenImmediately_RemovesAllChildren()
        {
            var parent = new GameObject("Parent");

            for (var i = 0; i < 4; i++)
            {
                new GameObject("Child" + i).transform.SetParent(parent.transform);
            }

            try
            {
                parent.transform.DestroyChildrenImmediately();
                Assert.AreEqual(0, parent.transform.childCount);
            }
            finally
            {
                Object.DestroyImmediate(parent);
            }
        }
    }
}
