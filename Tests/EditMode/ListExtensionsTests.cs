using System.Collections.Generic;
using NUnit.Framework;
using SOSXR.SeaShark;

//using Shuffle = SOSXR.SeaShark.Shuffle;

namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class ListExtensionsTests
    {
        // ── Clone ─────────────────────────────────────────────────────────────

        [Test]
        public void Clone_ReturnsNewListWithSameElements()
        {
            var original = new List<int> { 1, 2, 3 };
            var clone = original.Clone();

            Assert.AreNotSame(original, clone);
            Assert.AreEqual(original.Count, clone.Count);

            for (var i = 0; i < original.Count; i++)
            {
                Assert.AreEqual(original[i], clone[i]);
            }
        }

        [Test]
        public void Clone_ModifyingOriginal_DoesNotAffectClone()
        {
            var original = new List<int> { 1, 2, 3 };
            var clone = original.Clone();

            original.Add(4);

            Assert.AreEqual(3, clone.Count);
        }

        [Test]
        public void Clone_EmptyList_ReturnsEmptyList()
        {
            var empty = new List<string>();
            var clone = empty.Clone();

            Assert.IsNotNull(clone);
            Assert.AreEqual(0, clone.Count);
        }

        // ── Swap ──────────────────────────────────────────────────────────────

        [Test]
        public void Swap_SwapsElementsAtGivenIndices()
        {
            var list = new List<int> { 1, 2, 3 };
            list.Swap(0, 2);

            Assert.AreEqual(3, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(1, list[2]);
        }

        [Test]
        public void Swap_SameIndex_ListUnchanged()
        {
            var list = new List<string> { "a", "b", "c" };
            list.Swap(1, 1);

            Assert.AreEqual("b", list[1]);
        }

        // ── Filter ────────────────────────────────────────────────────────────

        [Test]
        public void Filter_MatchingPredicate_ReturnsMatchingElements()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var evens = list.Filter(x => x % 2 == 0);

            Assert.AreEqual(2, evens.Count);
            Assert.Contains(2, (System.Collections.IList)evens);
            Assert.Contains(4, (System.Collections.IList)evens);
        }

        [Test]
        public void Filter_NoMatch_ReturnsEmptyList()
        {
            var list = new List<int> { 1, 3, 5 };
            var result = list.Filter(x => x % 2 == 0);

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Filter_AllMatch_ReturnsAllElements()
        {
            var list = new List<int> { 2, 4, 6 };
            var result = list.Filter(x => x % 2 == 0);

            Assert.AreEqual(3, result.Count);
        }

        // ── Shuffle ───────────────────────────────────────────────────────────

        [Test]
        public void Shuffle_ReturnsSameListReference()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var result = list.Shuffle();

            Assert.AreSame(list, result);
        }

        [Test]
        public void Shuffle_RetainsAllElements()
        {
            var original = new List<int> { 1, 2, 3, 4, 5 };
            var copy = original.Clone();

            original.Shuffle();

            Assert.AreEqual(copy.Count, original.Count);

            foreach (var item in copy)
            {
                Assert.Contains(item, (System.Collections.IList)original);
            }
        }
    }
}
