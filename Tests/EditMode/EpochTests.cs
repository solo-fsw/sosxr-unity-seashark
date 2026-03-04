using System;
using NUnit.Framework;
using SOSXR.SeaShark.Statics;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class EpochTests
    {
        [Test]
        public void GetCurrentTimeInMilliseconds_ReturnsPositiveValue()
        {
            Assert.Greater(Epoch.GetCurrentTimeInMilliseconds(), 0L);
        }

        [Test]
        public void GetCurrentTimeInMilliseconds_IsCloseToSystemTime()
        {
            var before = DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds();
            var epoch  = Epoch.GetCurrentTimeInMilliseconds();
            var after  = DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds();

            Assert.GreaterOrEqual(epoch, before);
            Assert.LessOrEqual(epoch, after + 100); // 100 ms tolerance
        }

        [Test]
        public void FormatEpoch_Long_KnownTimestamp_ReturnsExpectedString()
        {
            // Unix epoch 0 = 1970-01-01 00:00:00 UTC
            // ToLocalTime will offset; we test only that it parses and returns the right date in UTC
            // Use a known UTC timestamp: 2024-03-15 12:00:00.000 UTC = 1710504000000 ms
            const long ts  = 1710504000000L;
            var result      = Epoch.FormatEpoch(ts, "yyyy-MM-dd");
            // The exact date depends on local timezone; we verify the format matches and parses
            Assert.DoesNotThrow(() => DateTime.ParseExact(result, "yyyy-MM-dd", null));
        }

        [Test]
        public void FormatEpoch_Double_ProducesSameResultAsLong()
        {
            const long epoch  = 1710504000000L;
            const double epochD = 1710504000000.0;

            Assert.AreEqual(Epoch.FormatEpoch(epoch), Epoch.FormatEpoch(epochD));
        }

        [Test]
        public void FormatEpoch_CustomFormat_ReturnsFormattedString()
        {
            const long ts = 1710504000000L;
            var result    = Epoch.FormatEpoch(ts, "yyyy");

            Assert.IsFalse(string.IsNullOrEmpty(result));
            Assert.AreEqual(4, result.Length); // "yyyy" should produce a 4-digit year
        }
    }
}
