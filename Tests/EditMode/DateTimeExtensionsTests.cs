using System;
using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        private readonly DateTime _base = new DateTime(2024, 6, 15, 10, 30, 45, 500);

        [Test]
        public void WithDate_ChangeYear_OnlyYearChanges()
        {
            var result = _base.WithDate(year: 2000);
            Assert.AreEqual(2000, result.Year);
            Assert.AreEqual(_base.Month, result.Month);
            Assert.AreEqual(_base.Day, result.Day);
        }

        [Test]
        public void WithDate_ChangeMonth_OnlyMonthChanges()
        {
            var result = _base.WithDate(month: 12);
            Assert.AreEqual(_base.Year, result.Year);
            Assert.AreEqual(12, result.Month);
            Assert.AreEqual(_base.Day, result.Day);
        }

        [Test]
        public void WithDate_ChangeDay_OnlyDayChanges()
        {
            var result = _base.WithDate(day: 1);
            Assert.AreEqual(_base.Year, result.Year);
            Assert.AreEqual(_base.Month, result.Month);
            Assert.AreEqual(1, result.Day);
        }

        [Test]
        public void WithDate_NoParams_PreservesEntireDate()
        {
            var result = _base.WithDate();
            Assert.AreEqual(_base.Year,  result.Year);
            Assert.AreEqual(_base.Month, result.Month);
            Assert.AreEqual(_base.Day,   result.Day);
        }

        [Test]
        public void WithDate_PreservesTimeComponents()
        {
            var result = _base.WithDate(year: 2020, month: 1, day: 1);
            Assert.AreEqual(_base.Hour,        result.Hour);
            Assert.AreEqual(_base.Minute,      result.Minute);
            Assert.AreEqual(_base.Second,      result.Second);
            Assert.AreEqual(_base.Millisecond, result.Millisecond);
        }

        [Test]
        public void WithDate_DayExceedsMonthDays_ClampsToDaysInMonth()
        {
            // January has 31 days; February does not — switch to Feb with day=31 → clamps to 28 (2024 is a leap year → 29)
            var jan31 = new DateTime(2023, 1, 31);
            var result = jan31.WithDate(month: 2); // 2023 is not a leap year → max 28
            Assert.AreEqual(2,  result.Month);
            Assert.AreEqual(28, result.Day);
        }

        [Test]
        public void WithDate_LeapYear_AllowsDay29InFebruary()
        {
            var jan31LeapYear = new DateTime(2024, 1, 31); // 2024 is a leap year
            var result = jan31LeapYear.WithDate(month: 2);
            Assert.AreEqual(2,  result.Month);
            Assert.AreEqual(29, result.Day); // leap year allows Feb 29
        }
    }
}
