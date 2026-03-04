using System;
using System.Collections.Generic;
using NUnit.Framework;
using SOSXR.SeaShark;


namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        // ── ForEach ───────────────────────────────────────────────────────────

        [Test]
        public void ForEach_CallsActionOnEachElement()
        {
            var list    = new List<int> {1, 2, 3};
            var visited = new List<int>();

            list.ForEach(x => visited.Add(x));

            Assert.AreEqual(3, visited.Count);
            Assert.AreEqual(1, visited[0]);
            Assert.AreEqual(2, visited[1]);
            Assert.AreEqual(3, visited[2]);
        }

        [Test]
        public void ForEach_EmptySequence_ActionNeverCalled()
        {
            var list       = new List<int>();
            var callCount  = 0;

            list.ForEach(_ => callCount++);

            Assert.AreEqual(0, callCount);
        }

        // ── Random ────────────────────────────────────────────────────────────

        [Test]
        public void Random_NullSequence_ThrowsArgumentNullException()
        {
            IEnumerable<int> seq = null;
            Assert.Throws<ArgumentNullException>(() => seq.Random());
        }

        [Test]
        public void Random_EmptyList_ThrowsInvalidOperationException()
        {
            var empty = new List<int>();
            Assert.Throws<InvalidOperationException>(() => empty.Random());
        }

        [Test]
        public void Random_SingleElement_ReturnsThatElement()
        {
            var list = new List<string> {"only"};
            Assert.AreEqual("only", list.Random());
        }

        [Test]
        public void Random_ReturnElementFromSequence()
        {
            var list    = new List<int> {10, 20, 30, 40, 50};
            var element = list.Random();
            Assert.Contains(element, list);
        }

        [Test]
        public void Random_NonListEnumerable_ReturnElementFromSequence()
        {
            // Uses reservoir sampling path
            IEnumerable<int> seq = GetSequence();
            var element = seq.Random();
            Assert.Contains(element, new List<int> {1, 2, 3, 4, 5});
        }

        private static IEnumerable<int> GetSequence()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            yield return 4;
            yield return 5;
        }
    }
}
