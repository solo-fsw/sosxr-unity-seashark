using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        // ── IsNullOrWhiteSpace ────────────────────────────────────────────────

        [Test]
        public void IsNullOrWhiteSpace_Null_ReturnsTrue()
        {
            string val = null;
            Assert.IsTrue(val.IsNullOrWhiteSpace());
        }

        [Test]
        public void IsNullOrWhiteSpace_Whitespace_ReturnsTrue()
        {
            Assert.IsTrue("   ".IsNullOrWhiteSpace());
        }

        [Test]
        public void IsNullOrWhiteSpace_NonEmpty_ReturnsFalse()
        {
            Assert.IsFalse("hello".IsNullOrWhiteSpace());
        }

        // ── IsNullOrEmpty ────────────────────────────────────────────────────

        [Test]
        public void IsNullOrEmpty_Null_ReturnsTrue()
        {
            string val = null;
            Assert.IsTrue(val.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_Empty_ReturnsTrue()
        {
            Assert.IsTrue(string.Empty.IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_NonEmpty_ReturnsFalse()
        {
            Assert.IsFalse("hello".IsNullOrEmpty());
        }

        [Test]
        public void IsNullOrEmpty_WhitespaceOnly_ReturnsFalse()
        {
            Assert.IsFalse("   ".IsNullOrEmpty());
        }

        // ── IsBlank ──────────────────────────────────────────────────────────

        [Test]
        public void IsBlank_Null_ReturnsTrue()
        {
            string val = null;
            Assert.IsTrue(val.IsBlank());
        }

        [Test]
        public void IsBlank_Empty_ReturnsTrue()
        {
            Assert.IsTrue(string.Empty.IsBlank());
        }

        [Test]
        public void IsBlank_Whitespace_ReturnsTrue()
        {
            Assert.IsTrue("   ".IsBlank());
        }

        [Test]
        public void IsBlank_NonEmpty_ReturnsFalse()
        {
            Assert.IsFalse("abc".IsBlank());
        }

        // ── OrEmpty ──────────────────────────────────────────────────────────

        [Test]
        public void OrEmpty_Null_ReturnsEmptyString()
        {
            string val = null;
            Assert.AreEqual(string.Empty, val.OrEmpty());
        }

        [Test]
        public void OrEmpty_NonNull_ReturnsSameString()
        {
            Assert.AreEqual("hello", "hello".OrEmpty());
        }

        [Test]
        public void OrEmpty_EmptyString_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, string.Empty.OrEmpty());
        }

        // ── Shorten ──────────────────────────────────────────────────────────

        [Test]
        public void Shorten_StringLongerThanMax_TruncatesAtMax()
        {
            Assert.AreEqual("hello", "hello world".Shorten(5));
        }

        [Test]
        public void Shorten_StringShorterThanMax_ReturnsFullString()
        {
            Assert.AreEqual("hi", "hi".Shorten(10));
        }

        [Test]
        public void Shorten_StringEqualToMax_ReturnsFullString()
        {
            Assert.AreEqual("hello", "hello".Shorten(5));
        }

        [Test]
        public void Shorten_BlankInput_ReturnsBlankUnchanged()
        {
            string val = null;
            Assert.IsNull(val.Shorten(5));
        }

        // ── Slice ────────────────────────────────────────────────────────────

        [Test]
        public void Slice_ValidRange_ReturnsSubstring()
        {
            Assert.AreEqual("ell", "hello".Slice(1, 4));
        }

        [Test]
        public void Slice_StartZero_ReturnsFromBeginning()
        {
            Assert.AreEqual("hel", "hello".Slice(0, 3));
        }

        [Test]
        public void Slice_NegativeEndIndex_CountsFromEnd()
        {
            Assert.AreEqual("hel", "hello".Slice(0, -2));
        }

        [Test]
        public void Slice_BlankInput_ThrowsArgumentNullException()
        {
            string val = null;
            Assert.Throws<System.ArgumentNullException>(() => val.Slice(0, 1));
        }

        [Test]
        public void Slice_NegativeStartIndex_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => "hello".Slice(-1, 3));
        }

        [Test]
        public void Slice_EndBeforeStart_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => "hello".Slice(3, 1));
        }

        // ── ConvertToAlphanumeric ─────────────────────────────────────────────

        [Test]
        public void ConvertToAlphanumeric_RemovesSpecialChars()
        {
            Assert.AreEqual("hello_world", "hello_world!@#".ConvertToAlphanumeric());
        }

        [Test]
        public void ConvertToAlphanumeric_EmptyInput_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, string.Empty.ConvertToAlphanumeric());
        }

        [Test]
        public void ConvertToAlphanumeric_NullInput_ReturnsEmpty()
        {
            Assert.AreEqual(string.Empty, ((string) null).ConvertToAlphanumeric());
        }

        [Test]
        public void ConvertToAlphanumeric_LeadingDigit_Stripped()
        {
            Assert.AreEqual("abc", "123abc".ConvertToAlphanumeric());
        }

        [Test]
        public void ConvertToAlphanumeric_AllowPeriods_KeepsPeriods()
        {
            Assert.AreEqual("com.example", "com.example".ConvertToAlphanumeric(allowPeriods: true));
        }

        [Test]
        public void ConvertToAlphanumeric_TrailingPeriod_Removed()
        {
            Assert.AreEqual("example", "example.".ConvertToAlphanumeric(allowPeriods: true));
        }

        [Test]
        public void ConvertToAlphanumeric_PeriodsDisallowed_RemovesPeriods()
        {
            Assert.AreEqual("comexample", "com.example".ConvertToAlphanumeric(allowPeriods: false));
        }

        // ── Rich Text helpers ────────────────────────────────────────────────

        [Test]
        public void RichColor_ReturnsColorTag()
        {
            Assert.AreEqual("<color=red>hello</color>", "hello".RichColor("red"));
        }

        [Test]
        public void RichSize_ReturnsSizeTag()
        {
            Assert.AreEqual("<size=24>hello</size>", "hello".RichSize(24));
        }

        [Test]
        public void RichBold_ReturnsBoldTag()
        {
            Assert.AreEqual("<b>hello</b>", "hello".RichBold());
        }

        [Test]
        public void RichItalic_ReturnsItalicTag()
        {
            Assert.AreEqual("<i>hello</i>", "hello".RichItalic());
        }

        [Test]
        public void RichUnderline_ReturnsUnderlineTag()
        {
            Assert.AreEqual("<u>hello</u>", "hello".RichUnderline());
        }

        [Test]
        public void RichStrikethrough_ReturnsStrikethroughTag()
        {
            Assert.AreEqual("<s>hello</s>", "hello".RichStrikethrough());
        }
    }
}
