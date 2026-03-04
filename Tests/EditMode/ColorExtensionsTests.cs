using NUnit.Framework;
using SOSXR.SeaShark;
using UnityEngine;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class ColorExtensionsTests
    {
        private static readonly Color Red   = new Color(1f, 0f, 0f, 1f);
        private static readonly Color Green = new Color(0f, 1f, 0f, 1f);
        private static readonly Color Blue  = new Color(0f, 0f, 1f, 1f);
        private static readonly Color Black = new Color(0f, 0f, 0f, 1f);
        private static readonly Color White = new Color(1f, 1f, 1f, 1f);

        // ── SetAlpha ──────────────────────────────────────────────────────────

        [Test]
        public void SetAlpha_SetsAlphaComponent()
        {
            var result = Red.SetAlpha(0.5f);
            Assert.AreEqual(1f,   result.r, 0.0001f);
            Assert.AreEqual(0f,   result.g, 0.0001f);
            Assert.AreEqual(0f,   result.b, 0.0001f);
            Assert.AreEqual(0.5f, result.a, 0.0001f);
        }

        [Test]
        public void SetAlpha_ZeroAlpha_MakesTransparent()
        {
            var result = Red.SetAlpha(0f);
            Assert.AreEqual(0f, result.a, 0.0001f);
        }

        // ── Invert ────────────────────────────────────────────────────────────

        [Test]
        public void Invert_Red_ReturnsInvertedRGB()
        {
            var result = Red.Invert();
            Assert.AreEqual(0f, result.r, 0.0001f);
            Assert.AreEqual(1f, result.g, 0.0001f);
            Assert.AreEqual(1f, result.b, 0.0001f);
            Assert.AreEqual(1f, result.a, 0.0001f, "Alpha should be preserved unchanged");
        }

        [Test]
        public void Invert_White_ReturnsBlack()
        {
            var result = White.Invert();
            Assert.AreEqual(0f, result.r, 0.0001f);
            Assert.AreEqual(0f, result.g, 0.0001f);
            Assert.AreEqual(0f, result.b, 0.0001f);
        }

        // ── Add ───────────────────────────────────────────────────────────────

        [Test]
        public void Add_ClampedAt1_NeverExceeds1()
        {
            var result = White.Add(White);
            Assert.LessOrEqual(result.r, 1f);
            Assert.LessOrEqual(result.g, 1f);
            Assert.LessOrEqual(result.b, 1f);
        }

        [Test]
        public void Add_RedAndGreen_YieldsYellow()
        {
            var result = Red.Add(Green);
            Assert.AreEqual(1f, result.r, 0.0001f);
            Assert.AreEqual(1f, result.g, 0.0001f);
            Assert.AreEqual(0f, result.b, 0.0001f);
        }

        // ── Subtract ──────────────────────────────────────────────────────────

        [Test]
        public void Subtract_ClampedAt0_NeverGoesNegative()
        {
            var result = Black.Subtract(White);
            Assert.GreaterOrEqual(result.r, 0f);
            Assert.GreaterOrEqual(result.g, 0f);
            Assert.GreaterOrEqual(result.b, 0f);
        }

        // ── Blend ─────────────────────────────────────────────────────────────

        [Test]
        public void Blend_At0_ReturnsFirstColor()
        {
            var result = Red.Blend(Blue, 0f);
            Assert.AreEqual(Red.r, result.r, 0.0001f);
            Assert.AreEqual(Red.b, result.b, 0.0001f);
        }

        [Test]
        public void Blend_At1_ReturnsSecondColor()
        {
            var result = Red.Blend(Blue, 1f);
            Assert.AreEqual(Blue.r, result.r, 0.0001f);
            Assert.AreEqual(Blue.b, result.b, 0.0001f);
        }

        [Test]
        public void Blend_At05_ReturnsMidpoint()
        {
            var result = Red.Blend(Blue, 0.5f);
            Assert.AreEqual(0.5f, result.r, 0.0001f);
            Assert.AreEqual(0f,   result.g, 0.0001f);
            Assert.AreEqual(0.5f, result.b, 0.0001f);
        }

        [Test]
        public void Blend_RatioClampedAbove1_StillReturnsSecondColor()
        {
            var result = Red.Blend(Blue, 2f);
            Assert.AreEqual(0f, result.r, 0.0001f);
            Assert.AreEqual(1f, result.b, 0.0001f);
        }
    }
}
