using NUnit.Framework;
using SOSXR.SeaShark;
using UnityEngine;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class VectorConversionExtensionsTests
    {
        // ── System.Numerics.Vector2 ↔ UnityEngine.Vector2 ────────────────────

        [Test]
        public void ToUnityVector_V2_ConvertsXAndY()
        {
            var sys    = new System.Numerics.Vector2(1.5f, 2.5f);
            var unity  = sys.ToUnityVector();
            Assert.AreEqual(1.5f, unity.x, 0.0001f);
            Assert.AreEqual(2.5f, unity.y, 0.0001f);
        }

        [Test]
        public void ToSystemVector_V2_ConvertsXAndY()
        {
            var unity = new Vector2(3f, 4f);
            var sys   = unity.ToSystemVector();
            Assert.AreEqual(3f, sys.X, 0.0001f);
            Assert.AreEqual(4f, sys.Y, 0.0001f);
        }

        [Test]
        public void RoundTrip_V2_SystemToUnityToSystem_PreservesValues()
        {
            var original = new System.Numerics.Vector2(7f, 8f);
            var roundtrip = original.ToUnityVector().ToSystemVector();
            Assert.AreEqual(original.X, roundtrip.X, 0.0001f);
            Assert.AreEqual(original.Y, roundtrip.Y, 0.0001f);
        }

        // ── System.Numerics.Vector3 ↔ UnityEngine.Vector3 ────────────────────

        [Test]
        public void ToUnityVector_V3_ConvertsXYAndZ()
        {
            var sys   = new System.Numerics.Vector3(1f, 2f, 3f);
            var unity = sys.ToUnityVector();
            Assert.AreEqual(1f, unity.x, 0.0001f);
            Assert.AreEqual(2f, unity.y, 0.0001f);
            Assert.AreEqual(3f, unity.z, 0.0001f);
        }

        [Test]
        public void ToSystemVector_V3_ConvertsXYAndZ()
        {
            var unity = new Vector3(5f, 6f, 7f);
            var sys   = unity.ToSystemVector();
            Assert.AreEqual(5f, sys.X, 0.0001f);
            Assert.AreEqual(6f, sys.Y, 0.0001f);
            Assert.AreEqual(7f, sys.Z, 0.0001f);
        }

        [Test]
        public void RoundTrip_V3_SystemToUnityToSystem_PreservesValues()
        {
            var original  = new System.Numerics.Vector3(10f, 20f, 30f);
            var roundtrip = original.ToUnityVector().ToSystemVector();
            Assert.AreEqual(original.X, roundtrip.X, 0.0001f);
            Assert.AreEqual(original.Y, roundtrip.Y, 0.0001f);
            Assert.AreEqual(original.Z, roundtrip.Z, 0.0001f);
        }
    }
}
