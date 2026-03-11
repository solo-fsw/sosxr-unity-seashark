using NUnit.Framework;
using SOSXR.SeaShark;
using UnityEngine;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class Vector2ExtensionsTests
    {
        private static readonly Vector2 Base = new Vector2(3f, 4f);

        // ── Add ───────────────────────────────────────────────────────────────

        [Test]
        public void Add_AddsToXAndY()
        {
            var result = Base.Add(x: 1f, y: 2f);
            Assert.AreEqual(4f, result.x, 0.0001f);
            Assert.AreEqual(6f, result.y, 0.0001f);
        }

        [Test]
        public void Add_DefaultsToZero_LeavesComponentsUnchanged()
        {
            var result = Base.Add();
            Assert.AreEqual(Base.x, result.x, 0.0001f);
            Assert.AreEqual(Base.y, result.y, 0.0001f);
        }

        // ── With ──────────────────────────────────────────────────────────────

        [Test]
        public void With_OnlyX_ChangesXPreservesY()
        {
            var result = Base.With(x: 10f);
            Assert.AreEqual(10f,   result.x, 0.0001f);
            Assert.AreEqual(Base.y, result.y, 0.0001f);
        }

        [Test]
        public void With_OnlyY_ChangesYPreservesX()
        {
            var result = Base.With(y: 99f);
            Assert.AreEqual(Base.x, result.x, 0.0001f);
            Assert.AreEqual(99f,    result.y, 0.0001f);
        }

        [Test]
        public void With_BothNull_ReturnsUnchanged()
        {
            var result = Base.With();
            Assert.AreEqual(Base.x, result.x, 0.0001f);
            Assert.AreEqual(Base.y, result.y, 0.0001f);
        }

        // ── InRangeOf ─────────────────────────────────────────────────────────

        [Test]
        public void InRangeOf_SamePoint_IsInRange()
        {
            Assert.IsTrue(Base.InRangeOf(Base, 0f));
        }

        [Test]
        public void InRangeOf_WithinRange_ReturnsTrue()
        {
            var nearby = new Vector2(3.1f, 4.1f);
            Assert.IsTrue(Base.InRangeOf(nearby, 1f));
        }

        [Test]
        public void InRangeOf_OutsideRange_ReturnsFalse()
        {
            var farAway = new Vector2(100f, 100f);
            Assert.IsFalse(Base.InRangeOf(farAway, 1f));
        }
    }
}
