using System;
using NUnit.Framework;
using SOSXR.SeaShark.Excessives;
using SOSXR.SeaShark.Statics;
using UnityEngine;


namespace SOSXR.SeaShark.Tests
{
    /// <summary>
    ///     Tests for pure-logic behaviour fixed in the 2026-11-09 code review.
    /// </summary>
    [TestFixture]
    public class RuntimeFixTests
    {
        #region QueryURL NormalizeName

        private class NamingData
        {
#pragma warning disable CS0414
            private string m_unity = "u1";
            private string _under = "u2";
            private string mood = "happy";
            public string Plain = "p1";
#pragma warning restore CS0414
        }


        private const string BaseURL = "https://example.com/q";


        [Test]
        public void QueryURL_MPrefix_IsStripped()
        {
            var url = new NamingData().BuildQueryURL(BaseURL, "unity");

            Assert.AreEqual(BaseURL + "?unity=u1", url);
        }


        [Test]
        public void QueryURL_UnderscorePrefix_IsStripped()
        {
            var url = new NamingData().BuildQueryURL(BaseURL, "under");

            Assert.AreEqual(BaseURL + "?under=u2", url);
        }


        [Test]
        public void QueryURL_PlainName_IsUntouched()
        {
            var url = new NamingData().BuildQueryURL(BaseURL, "Plain");

            Assert.AreEqual(BaseURL + "?Plain=p1", url);
        }


        [Test]
        public void QueryURL_NameStartingWithM_IsNotMangled()
        {
            var url = new NamingData().BuildQueryURL(BaseURL, "mood");

            Assert.AreEqual(BaseURL + "?mood=happy", url);
        }

        #endregion


        #region GlobalEvent

        public class TestEvent : GlobalEvent<TestEvent>, IDisposable
        {
            public int Value;


            protected override void Reset()
            {
                Value = 0;
            }
        }


        private TestEvent.EventListener _listener;


        [TearDown]
        public void TearDownGlobalEvent()
        {
            if (_listener != null)
            {
                TestEvent.UnregisterListener(_listener);
                _listener = null;
            }
        }


        [Test]
        public void GlobalEvent_GetTwiceWithoutFiring_Throws()
        {
            var first = TestEvent.Get();

            try
            {
                Assert.Throws<Exception>(() => TestEvent.Get());
                Assert.IsTrue(TestEvent.InUse);
            }
            finally
            {
                first.Dispose();
            }

            Assert.IsFalse(TestEvent.InUse);
        }


        [Test]
        public void GlobalEvent_AfterDispose_GetWorksAgain()
        {
            using (var e = TestEvent.Get())
            {
                e.Value = 5;
            }

            Assert.IsFalse(TestEvent.InUse);

            var again = TestEvent.Get();

            try
            {
                Assert.IsNotNull(again);
                Assert.AreEqual(0, again.Value, "Reset() should have cleared data");
            }
            finally
            {
                again.Dispose();
            }
        }


        [Test]
        public void GlobalEvent_AfterFireEvent_GetWorksAgain()
        {
            var e = TestEvent.Get();
            e.FireEvent();

            Assert.IsFalse(TestEvent.InUse);

            var again = TestEvent.Get();
            again.FireEvent();

            Assert.IsFalse(TestEvent.InUse);
        }


        [Test]
        public void GlobalEvent_FireTwice_Throws()
        {
            var e = TestEvent.Get();
            e.FireEvent();

            Assert.Throws<Exception>(() => e.FireEvent());
        }


        [Test]
        public void GlobalEvent_ListenerReceivesData()
        {
            var received = -1;
            _listener = info => received = info.Value;
            TestEvent.RegisterListener(_listener);

            using (var e = TestEvent.Get())
            {
                e.Value = 42;
            }

            Assert.AreEqual(42, received);
        }


        [Test]
        public void GlobalEvent_ThrowingListener_PropagatesAndReleasesInUse()
        {
            _listener = _ => throw new InvalidOperationException("boom");
            TestEvent.RegisterListener(_listener);

            var e = TestEvent.Get();

            Assert.Throws<InvalidOperationException>(() => e.FireEvent());
            Assert.IsFalse(TestEvent.InUse);

            // Event must be usable again.
            TestEvent.UnregisterListener(_listener);
            _listener = null;

            var again = TestEvent.Get();
            again.FireEvent();
            Assert.IsFalse(TestEvent.InUse);
        }

        #endregion


        #region PID

        [Test]
        public void PID_DefaultDeltaTime_IsOne()
        {
            var pid = new PID(1, 1, 1);

            // error 4: I=4, D=(4-0)/1=4, P=4 -> 4+4+4
            Assert.AreEqual(12d, pid.Step(4), 1e-9);
            Assert.AreEqual(4d, pid.I, 1e-9);
            Assert.AreEqual(4d, pid.D, 1e-9);
            Assert.AreEqual(4d, pid.P, 1e-9);
        }


        [Test]
        public void PID_Derivative_IsErrorMinusPreviousOverDt()
        {
            var pid = new PID(0, 0, 1);

            pid.Step(2, 0.5);
            var output = pid.Step(5, 0.5); // (5 - 2) / 0.5 = 6

            Assert.AreEqual(6d, pid.D, 1e-9);
            Assert.AreEqual(6d, output, 1e-9);
        }


        [Test]
        public void PID_Derivative_NegativeWhenErrorDecreases()
        {
            var pid = new PID(0, 0, 1);

            pid.Step(5);
            pid.Step(2);

            Assert.AreEqual(-3d, pid.D, 1e-9);
        }


        [Test]
        public void PID_Integral_AccumulatesErrorTimesDt()
        {
            var pid = new PID(0, 1, 0);

            pid.Step(2, 0.5); // 1
            var output = pid.Step(4, 0.25); // +1 = 2

            Assert.AreEqual(2d, pid.I, 1e-9);
            Assert.AreEqual(2d, output, 1e-9);
        }


        [Test]
        public void PID_NonPositiveDeltaTime_FallsBackToOne()
        {
            var pid = new PID(0, 1, 1);

            pid.Step(3, 0d); // dt -> 1: I=3, D=3

            Assert.AreEqual(3d, pid.I, 1e-9);
            Assert.AreEqual(3d, pid.D, 1e-9);
        }

        #endregion


        #region ExtensionsE IsNull / NotNull

        private class Plain
        {
        }


        [Test]
        public void IsNull_PlainObject_NullAndNonNull()
        {
            Plain nothing = null;

            Assert.IsTrue(nothing.IsNull());
            Assert.IsFalse(nothing.NotNull());
            Assert.IsFalse(new Plain().IsNull());
            Assert.IsTrue(new Plain().NotNull());
        }


        [Test]
        public void IsNull_DestroyedGameObject_IsTreatedAsNull()
        {
            var go = new GameObject("destroyed");
            UnityEngine.Object.DestroyImmediate(go);

            Assert.IsTrue(go.IsNull());
            Assert.IsFalse(go.NotNull());
        }


        [Test]
        public void IsNull_LiveGameObject_IsNotNull()
        {
            var go = new GameObject("live");

            try
            {
                Assert.IsFalse(go.IsNull());
                Assert.IsTrue(go.NotNull());
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }


        [Test]
        public void IsNullAction_DestroyedObject_InvokesActionAndReturnsTrue()
        {
            var go = new GameObject("destroyed");
            UnityEngine.Object.DestroyImmediate(go);
            var called = false;

            var result = go.IsNull(_ => called = true);

            Assert.IsTrue(result);
            Assert.IsTrue(called);
        }


        [Test]
        public void IsNullAction_LiveObject_DoesNotInvokeAction()
        {
            var go = new GameObject("live");

            try
            {
                var called = false;
                var result = go.IsNull(_ => called = true);

                Assert.IsFalse(result);
                Assert.IsFalse(called);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }


        [Test]
        public void NotNullAction_DestroyedObject_DoesNotInvokeAction()
        {
            var go = new GameObject("destroyed");
            UnityEngine.Object.DestroyImmediate(go);
            var called = false;

            var result = go.NotNull(_ => called = true);

            Assert.IsFalse(result);
            Assert.IsFalse(called);
        }


        [Test]
        public void NotNullAction_LiveObject_InvokesActionWithInstance()
        {
            var go = new GameObject("live");

            try
            {
                GameObject received = null;
                var result = go.NotNull(g => received = g);

                Assert.IsTrue(result);
                Assert.AreSame(go, received);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }


        [Test]
        public void NullActions_PlainObjects_Work()
        {
            Plain nothing = null;
            var nullCalled = false;
            var notNullCalled = false;

            Assert.IsTrue(nothing.IsNull(_ => nullCalled = true));
            Assert.IsFalse(nothing.NotNull(_ => notNullCalled = true));
            Assert.IsTrue(nullCalled);
            Assert.IsFalse(notNullCalled);

            var obj = new Plain();
            nullCalled = false;

            Assert.IsFalse(obj.IsNull(_ => nullCalled = true));
            Assert.IsTrue(obj.NotNull(_ => notNullCalled = true));
            Assert.IsFalse(nullCalled);
            Assert.IsTrue(notNullCalled);
        }

        #endregion


        #region Mediator

        private const string ChannelPrefix = "RuntimeFixTests_";
        private int _hits;


        private void CountHit(Medium m)
        {
            _hits++;
        }


        [SetUp]
        public void SetUpMediator()
        {
            // Mediator logs an error when no MediatorRegistry is in Resources.
            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;
            _hits = 0;
        }


        [TearDown]
        public void TearDownMediator()
        {
            var stale = new System.Collections.Generic.List<string>();

            foreach (var key in Mediator.Mediations.Keys)
            {
                if (key.StartsWith(ChannelPrefix, StringComparison.Ordinal))
                {
                    stale.Add(key);
                }
            }

            foreach (var key in stale)
            {
                Mediator.Mediations.Remove(key);
            }

            // Subscribe/Publish also write into the in-memory registry asset when present; drop our entries.
            var registry = Resources.Load<MediatorRegistry>(nameof(MediatorRegistry));

            if (registry != null)
            {
                registry.Registry.RemoveAll(i => i.Medium != null && i.Medium.Channel != null && i.Medium.Channel.StartsWith(ChannelPrefix, StringComparison.Ordinal));
            }

            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;
        }


        [Test]
        public void Mediator_SubscribePublish_InvokesListener()
        {
            var medium = new Medium(ChannelPrefix + "A");
            Mediator.Subscribe(medium, CountHit);

            Mediator.Publish(medium);

            Assert.AreEqual(1, _hits);
        }


        [Test]
        public void Mediator_Publish_PassesMediumToListener()
        {
            var medium = new Medium(ChannelPrefix + "B");
            Medium received = null;
            Mediator.Subscribe(medium, m => received = m);

            Mediator.Publish(medium);

            Assert.AreSame(medium, received);
        }


        [Test]
        public void Mediator_Unsubscribe_StopsInvocationAndRemovesChannel()
        {
            var medium = new Medium(ChannelPrefix + "C");
            Mediator.Subscribe(medium, CountHit);
            Mediator.Unsubscribe(medium, CountHit);

            Mediator.Publish(medium);

            Assert.AreEqual(0, _hits);
            Assert.IsFalse(Mediator.Mediations.ContainsKey(medium.Channel));
        }


        [Test]
        public void Mediator_UnsubscribeOneOfTwo_KeepsOther()
        {
            var medium = new Medium(ChannelPrefix + "D");
            var otherHits = 0;
            Action<Medium> other = _ => otherHits++;
            Mediator.Subscribe(medium, CountHit);
            Mediator.Subscribe(medium, other);

            Mediator.Unsubscribe(medium, CountHit);
            Mediator.Publish(medium);

            Assert.AreEqual(0, _hits);
            Assert.AreEqual(1, otherHits);
            Assert.IsTrue(Mediator.Mediations.ContainsKey(medium.Channel));
        }


        [Test]
        public void Mediator_UnsubscribeUnknownChannel_DoesNotThrow()
        {
            var medium = new Medium(ChannelPrefix + "E");

            Assert.DoesNotThrow(() => Mediator.Unsubscribe(medium, CountHit));
        }


        [Test]
        public void Mediator_SubscribeToEmptyMedium_IsIgnored()
        {
            var medium = new Medium(MediatorRegistry.EmptyName);

            Mediator.Subscribe(medium, CountHit);

            Assert.IsFalse(Mediator.Mediations.ContainsKey(MediatorRegistry.EmptyName));
        }

        #endregion


        #region TransformExtensions.Reset

        [Test]
        public void TransformReset_ResetsLocalPositionNotWorld()
        {
            var parent = new GameObject("parent");
            var child = new GameObject("child");

            try
            {
                parent.transform.position = new Vector3(5f, 6f, 7f);
                child.transform.SetParent(parent.transform, false);
                child.transform.localPosition = new Vector3(1f, 2f, 3f);
                child.transform.localScale = new Vector3(2f, 2f, 2f);
                child.transform.localRotation = Quaternion.Euler(10f, 20f, 30f);

                child.transform.Reset();

                Assert.AreEqual(Vector3.zero, child.transform.localPosition);
                Assert.AreEqual(Vector3.one, child.transform.localScale);
                Assert.AreEqual(Quaternion.identity, child.transform.localRotation);
                Assert.AreEqual(parent.transform.position, child.transform.position);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(child);
                UnityEngine.Object.DestroyImmediate(parent);
            }
        }

        #endregion


        #region Epoch

        [Test]
        public void Epoch_GetCurrentTimeAsString_YearFormat_IsCurrentYear()
        {
            var result = Epoch.GetCurrentTimeAsString("yyyy");

            Assert.AreEqual(4, result.Length);
            Assert.IsTrue(int.TryParse(result, out var year));
            Assert.That(Math.Abs(year - DateTime.Now.Year), Is.LessThanOrEqualTo(1));
        }


        [Test]
        public void Epoch_GetCurrentTimeAsString_DefaultFormat_IsParseable()
        {
            var result = Epoch.GetCurrentTimeAsString();

            Assert.IsTrue(DateTime.TryParseExact(result, "dd/MM/yyyy HH:mm:ss.fff",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out _), result);
        }


        [Test]
        public void Epoch_GetCurrentTimeAsString_CustomFormat_IsParseableAndRecent()
        {
            var result = Epoch.GetCurrentTimeAsString("yyyy-MM-dd HH:mm:ss");

            Assert.IsTrue(DateTime.TryParseExact(result, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsed), result);
            Assert.That(Math.Abs((DateTime.Now - parsed).TotalSeconds), Is.LessThan(120));
        }

        #endregion
    }
}
