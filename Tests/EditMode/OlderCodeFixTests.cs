using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using SOSXR.SeaShark.Excessives;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    ///     Regression tests for the fixes from the audit of older SeaShark code (2026-11-17).
    /// </summary>
    [TestFixture]
    public class OlderCodeFixTests
    {
        private readonly List<Object> _created = new();


        [TearDown]
        public void TearDown()
        {
            foreach (var obj in _created)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }

            _created.Clear();
        }


        private GameObject Create(string name)
        {
            var go = new GameObject(name);
            _created.Add(go);

            return go;
        }


        private static void WithCulture(string cultureName, Action action)
        {
            var previous = Thread.CurrentThread.CurrentCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = previous;
            }
        }


        // ── ChangeLayer ──────────────────────────────────────────────────────

        [TestCase(1, 0)]
        [TestCase(1 << 5, 5)]
        [TestCase(1 << 31, 31)]
        [TestCase(0, -1)]
        [TestCase((1 << 3) | (1 << 4), -1)]
        public void ChangeLayer_LayerFromMask(int mask, int expected)
        {
            Assert.AreEqual(expected, ChangeLayer.LayerFromMask(mask));
        }


        // ── FrameRateLimiter ─────────────────────────────────────────────────

        [TestCase(1, 10)]
        [TestCase(4, 60)]
        [TestCase(5, 900)]
        [TestCase(6, 0)]
        public void FrameRateLimiter_FrameRateForFunctionKey(int functionKey, int expected)
        {
            Assert.AreEqual(expected, FrameRateLimiter.FrameRateForFunctionKey(functionKey));
        }


        // ── CommandQueue ─────────────────────────────────────────────────────

        [Test]
        public void CommandQueue_Clear_AlsoClearsRedo()
        {
            var queue = new CommandQueue();
            var command = new CountingCommand();

            queue.Execute(command);
            queue.Undo();
            queue.Clear();
            queue.Redo();

            Assert.AreEqual(1, command.ExecuteCount, "Redo after Clear must not run the old command again.");
        }


        // ── TweenSequence ────────────────────────────────────────────────────

        [Test]
        public void TweenSequence_SecondLoop_LaterTweenDoesNotOverwriteEarlierOne()
        {
            var target = Create("TweenTarget").transform;
            target.localPosition = Vector3.zero;

            var sequence = new TweenSequence();
            sequence.Insert(0f, target.DOLocalMove(Vector3.right, 1f));
            sequence.Insert(1f, target.DOLocalMove(Vector3.up, 1f));
            sequence.SetLoops(2, LoopType.Restart);

            sequence.Advance(0.5f);
            AssertVector(new Vector3(0.5f, 0f, 0f), target.localPosition);

            sequence.Advance(1f); // 1.5: second tween halfway, from the end of the first
            AssertVector(new Vector3(0.5f, 0.5f, 0f), target.localPosition);

            sequence.Advance(0.6f); // 2.1: second loop, 0.1 into the first tween
            AssertVector(new Vector3(0.1f, 0f, 0f), target.localPosition);
        }


        [Test]
        public void TweenSequence_InsertedOutOfOrder_LatestStartedTweenWins()
        {
            var target = Create("TweenTarget").transform;
            target.localPosition = Vector3.zero;

            var sequence = new TweenSequence();
            sequence.Insert(1f, target.DOLocalMove(Vector3.up, 1f)); // inserted first, starts later
            sequence.Insert(0f, target.DOLocalMove(Vector3.right, 1f));

            sequence.Advance(1.5f);

            AssertVector(new Vector3(0.5f, 0.5f, 0f), target.localPosition);
        }


        private static void AssertVector(Vector3 expected, Vector3 actual)
        {
            Assert.That(Vector3.Distance(expected, actual), Is.LessThan(1e-3f), $"Expected {expected}, got {actual}");
        }


        // ── QueryURL ─────────────────────────────────────────────────────────

        private class QuerySource
        {
            public float Value = 1.5f;
        }


        [TestCase("https://example.com", "?")]
        [TestCase("https://example.com?a=1", "&")]
        [TestCase("https://example.com?", "")]
        [TestCase("https://example.com?a=1&", "")]
        public void QueryURL_QuerySeparator(string baseURL, string expected)
        {
            Assert.AreEqual(expected, QueryURL.QuerySeparator(baseURL));
        }


        [Test]
        public void QueryURL_Float_UsesInvariantCulture()
        {
            WithCulture("nl-NL", () =>
            {
                var url = new QuerySource().BuildQueryURL("https://example.com", "Value");

                Assert.AreEqual("https://example.com?Value=1.5", url);
            });
        }


        [Test]
        public void QueryURL_BaseUrlWithQuery_AppendsWithAmpersand()
        {
            var url = new QuerySource().BuildQueryURL("https://example.com?id=7", "Value");

            Assert.AreEqual("https://example.com?id=7&Value=1.5", url);
        }


        // ── StringExtensions ─────────────────────────────────────────────────

        [Test]
        public void RichRotationAndSpace_UseInvariantCulture()
        {
            WithCulture("nl-NL", () =>
            {
                Assert.AreEqual("<rotate=1.5>x</rotate>", "x".RichRotation(1.5f));
                Assert.AreEqual("<space=2.25>x</space>", "x".RichSpace(2.25f));
            });
        }


        // ── ExtensionMethods ─────────────────────────────────────────────────

        [Test]
        public void AddIfUnique_Null_IsAddedOnce()
        {
            var list = new List<string>();

            list.AddIfUnique(null);
            list.AddIfUnique(null);

            Assert.AreEqual(1, list.Count);
        }


        [Test]
        public void AddRangeUnique_NullItems_AddsNothing()
        {
            var list = new List<string> {"a"};

            list.AddRangeUnique(null);

            CollectionAssert.AreEqual(new[] {"a"}, list);
        }


        [TestCase(10, 3, 1)]
        [TestCase(5, 6, 5)]
        [TestCase(-7, 3, -1)]
        [TestCase(-2, 3, -2)]
        public void GetRemainder_MatchesModuloOperator(int dividend, int divisor, int expected)
        {
            Assert.AreEqual(expected, dividend.GetRemainder(divisor));
        }


        [Test]
        public void RoundCorrectly_Double_KeepsPrecisionAndRoundsMidpointAway()
        {
            Assert.AreEqual(2.68d, 2.675d.RoundCorrectly(2), 1e-12);
            Assert.AreEqual(123456789.123457d, 123456789.1234567d.RoundCorrectly(6), 1e-9);
        }


        [Test]
        public void RoundCorrectly_NaNAndInfinity_ReturnedUnchanged()
        {
            Assert.IsTrue(double.IsNaN(double.NaN.RoundCorrectly(2)));
            Assert.IsTrue(float.IsNaN(float.NaN.RoundCorrectly(2)));
            Assert.AreEqual(float.PositiveInfinity, float.PositiveInfinity.RoundCorrectly(2));
            Assert.AreEqual(float.MaxValue, float.MaxValue.RoundCorrectly(2));
        }


        [Test]
        public void GetChildrenWithoutParent_IncludesInactiveChildren_NoNulls()
        {
            var parent = Create("Parent").transform;
            var active = new GameObject("Active").transform;
            var inactive = new GameObject("Inactive").transform;
            active.SetParent(parent);
            inactive.SetParent(parent);
            inactive.gameObject.SetActive(false);

            var children = parent.GetChildrenWithoutParent();

            CollectionAssert.AreEqual(new[] {active, inactive}, children);
        }


        [Test]
        public void SortByName_WithDestroyedObject_DoesNotThrow()
        {
            var b = Create("B");
            var a = Create("A");
            var destroyed = new GameObject("Destroyed");
            Object.DestroyImmediate(destroyed);

            var list = new List<GameObject> {b, destroyed, a};

            Assert.DoesNotThrow(() => list.SortByName());
            Assert.AreEqual(a, list[1]);
            Assert.AreEqual(b, list[2]);
        }


        [Test]
        public void IsLastChild_RootObject_DoesNotThrow()
        {
            Create("RootOne");
            var last = Create("RootTwo").transform;

            Assert.IsTrue(last.IsLastChild());
        }


        // ── BitWisE ──────────────────────────────────────────────────────────

        [Test]
        public void BitWisE_BoolsToByteAndBack_RoundTrips()
        {
            var bits = new[] {true, false, false, true, false, true, false, false};

            var bytes = BitWisE.BoolArrayToSingleBinaryByte(bits);

            CollectionAssert.AreEqual(bits, BitWisE.SingleBinaryByteToBool(bytes));
        }


        // ── CryptoSymmetric ──────────────────────────────────────────────────

        [Test]
        public void CryptoSymmetric_EmptyKey_Throws()
        {
            Assert.Throws<ArgumentException>(() => CryptoSymmetric.Encrypt(new byte[] {1, 2, 3}, Array.Empty<byte>()));
        }


        [Test]
        public void CryptoSymmetric_EncryptThenDecrypt_RoundTrips()
        {
            var data = new byte[] {10, 20, 30, 40, 50};
            var key = new byte[] {7, 9};

            CollectionAssert.AreEqual(data, CryptoSymmetric.Decrypt(CryptoSymmetric.Encrypt(data, key), key));
        }


        // ── Move ─────────────────────────────────────────────────────────────

        [Test]
        public void Move_SmoothingFactor_IsFrameRateIndependent()
        {
            var oneStep = Move.SmoothingFactor(3f, 0.1f);
            var half = Move.SmoothingFactor(3f, 0.05f);
            var twoHalfSteps = 1f - (1f - half) * (1f - half);

            Assert.AreEqual(oneStep, twoHalfSteps, 1e-5f);
            Assert.AreEqual(0f, Move.SmoothingFactor(0f, 0.1f));
            Assert.That(Move.SmoothingFactor(1000f, 1f), Is.LessThanOrEqualTo(1f));
        }


        // ── WaitFor ──────────────────────────────────────────────────────────

        [Test]
        public void WaitFor_Seconds_ZeroOrNegative_ReturnsNull()
        {
            Assert.IsNull(WaitFor.Seconds(0f));
            Assert.IsNull(WaitFor.Seconds(-1f));
        }


        [Test]
        public void WaitFor_Seconds_SameDuration_ReturnsCachedInstance()
        {
            Assert.AreSame(WaitFor.Seconds(0.25f), WaitFor.Seconds(0.25f));
        }


        // ── SceneObjects ─────────────────────────────────────────────────────

        [Test]
        public void SceneObjects_FindAll_FindsCreatedObject()
        {
            var created = Create("SceneObjectsProbe").AddComponent<BoxCollider>();

            CollectionAssert.Contains(SceneObjects.FindAll<BoxCollider>(), created);
        }
    }
}
