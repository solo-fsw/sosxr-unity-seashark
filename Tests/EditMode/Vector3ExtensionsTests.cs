using NUnit.Framework;
using SOSXR.SeaShark;
using UnityEngine;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class Vector3ExtensionsTests
    {
        private static readonly Vector3 One = new Vector3(1f, 2f, 3f);

        // ── With ──────────────────────────────────────────────────────────────

        [Test]
        public void With_OnlyX_ChangesXPreservesYZ()
        {
            var result = One.With(x: 10f);
            Assert.AreEqual(10f, result.x, 0.0001f);
            Assert.AreEqual(2f,  result.y, 0.0001f);
            Assert.AreEqual(3f,  result.z, 0.0001f);
        }

        [Test]
        public void With_OnlyY_ChangesYPreservesXZ()
        {
            var result = One.With(y: 20f);
            Assert.AreEqual(1f,  result.x, 0.0001f);
            Assert.AreEqual(20f, result.y, 0.0001f);
            Assert.AreEqual(3f,  result.z, 0.0001f);
        }

        [Test]
        public void With_OnlyZ_ChangesZPreservesXY()
        {
            var result = One.With(z: 30f);
            Assert.AreEqual(1f,  result.x, 0.0001f);
            Assert.AreEqual(2f,  result.y, 0.0001f);
            Assert.AreEqual(30f, result.z, 0.0001f);
        }

        [Test]
        public void With_AllComponents_SetsAll()
        {
            var result = One.With(x: 5f, y: 6f, z: 7f);
            Assert.AreEqual(5f, result.x, 0.0001f);
            Assert.AreEqual(6f, result.y, 0.0001f);
            Assert.AreEqual(7f, result.z, 0.0001f);
        }

        // ── Add ───────────────────────────────────────────────────────────────

        [Test]
        public void Add_AddsToEachComponent()
        {
            var result = One.Add(x: 1f, y: 2f, z: 3f);
            Assert.AreEqual(2f, result.x, 0.0001f);
            Assert.AreEqual(4f, result.y, 0.0001f);
            Assert.AreEqual(6f, result.z, 0.0001f);
        }

        [Test]
        public void Add_DefaultsToZero_LeavesComponentsUnchanged()
        {
            var result = One.Add();
            Assert.AreEqual(One.x, result.x, 0.0001f);
            Assert.AreEqual(One.y, result.y, 0.0001f);
            Assert.AreEqual(One.z, result.z, 0.0001f);
        }

        // ── InRangeOf ─────────────────────────────────────────────────────────

        [Test]
        public void InRangeOf_SamePoint_IsInRange()
        {
            Assert.IsTrue(One.InRangeOf(One, 0f));
        }

        [Test]
        public void InRangeOf_WithinRange_ReturnsTrue()
        {
            var target = new Vector3(1f, 2f, 3.5f);
            Assert.IsTrue(One.InRangeOf(target, 1f));
        }

        [Test]
        public void InRangeOf_OutsideRange_ReturnsFalse()
        {
            var farAway = new Vector3(100f, 100f, 100f);
            Assert.IsFalse(One.InRangeOf(farAway, 1f));
        }

        // ── ComponentDivide ───────────────────────────────────────────────────

        [Test]
        public void ComponentDivide_DividesEachComponent()
        {
            var a = new Vector3(4f, 6f, 8f);
            var b = new Vector3(2f, 3f, 4f);
            var result = a.ComponentDivide(b);
            Assert.AreEqual(2f, result.x, 0.0001f);
            Assert.AreEqual(2f, result.y, 0.0001f);
            Assert.AreEqual(2f, result.z, 0.0001f);
        }

        [Test]
        public void ComponentDivide_DivisionByZero_PreservesOriginalComponent()
        {
            var a = new Vector3(4f, 6f, 8f);
            var b = new Vector3(0f, 3f, 0f);
            var result = a.ComponentDivide(b);
            Assert.AreEqual(4f, result.x, 0.0001f, "x: dividing by 0 should preserve original");
            Assert.AreEqual(2f, result.y, 0.0001f);
            Assert.AreEqual(8f, result.z, 0.0001f, "z: dividing by 0 should preserve original");
        }

        // ── ToVector3 (from Vector2) ──────────────────────────────────────────

        [Test]
        public void ToVector3_ConvertsV2ToV3WithZeroY()
        {
            var v2     = new Vector2(3f, 5f);
            var result = v2.ToVector3();
            Assert.AreEqual(3f, result.x, 0.0001f);
            Assert.AreEqual(0f, result.y, 0.0001f);
            Assert.AreEqual(5f, result.z, 0.0001f);
        }
    }
}
