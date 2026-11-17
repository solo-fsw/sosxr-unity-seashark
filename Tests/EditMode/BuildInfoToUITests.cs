using System;
using NUnit.Framework;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class BuildInfoToUITests
    {
        private const string Header = "PackageName, UnityVersion, TotalAttemptedBuilds, BuildTarget, ScriptingBackend, APICompatibility, ProductionBuild, SemVer, AndroidBundleCode, BuildDate, BuildTime, BuildDurationSec, BuildSizeMB, TotalSuccessBuilds, Result";
        private const string PreBuildLine = "com.sosxr.app, 6000.6.0f1, 12, Android, IL2CPP, NET_Unity_4_8, True, 0_1_5, 42, 2026-11-15, 14:30";


        [Test]
        public void FormatBuildInfo_ReadsColumnsFromEditorSpiceLayout()
        {
            var now = new DateTime(2026, 11, 16, 9, 5, 0);
            var line = PreBuildLine + ", 120, 104.6, 10, Succeeded";

            var text = BuildInfoToUI.FormatBuildInfo(line, now);

            StringAssert.Contains("Name: com.sosxr.app", text);
            StringAssert.Contains("ProductionBuild: True", text);
            StringAssert.Contains("SemVer: 0_1_5", text);
            StringAssert.Contains("BundleCode: 42", text);
            StringAssert.Contains("Build Date: yesterday at 14:30", text);
        }


        [Test]
        public void FormatBuildInfo_AcceptsPreBuildOnlyLine()
        {
            var text = BuildInfoToUI.FormatBuildInfo(PreBuildLine, new DateTime(2026, 11, 15, 15, 0, 0));

            StringAssert.Contains("Build Date: today at 14:30", text);
        }


        [Test]
        public void FormatBuildInfo_TooFewColumns_ReturnsInvalidText()
        {
            Assert.AreEqual("Invalid build information format.", BuildInfoToUI.FormatBuildInfo("a, b, c, d, e, f, g", DateTime.Now));
        }


        [Test]
        public void FormatBuildInfo_BadDate_ReturnsInvalidTextWithoutThrowing()
        {
            var line = "com.sosxr.app, 6000.6.0f1, 12, Android, IL2CPP, NET_4_6, True, 0_1_5, 42, not-a-date, 14:30";

            Assert.AreEqual("Invalid build information format.", BuildInfoToUI.FormatBuildInfo(line, DateTime.Now));
        }


        [Test]
        public void ReadLastLineOfFile_SkipsTrailingEmptyLines()
        {
            var content = Header + "\n" + PreBuildLine + "\n\n   \n";

            Assert.AreEqual(PreBuildLine, BuildInfoToUI.ReadLastLineOfFile(content));
        }


        [Test]
        public void ReadLastLineOfFile_EmptyContent_ReturnsNull()
        {
            Assert.IsNull(BuildInfoToUI.ReadLastLineOfFile("\n\n"));
        }
    }
}
