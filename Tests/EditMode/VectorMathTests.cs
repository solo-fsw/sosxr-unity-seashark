using NUnit.Framework;
using SOSXR.SeaShark;
using UnityEngine;

namespace SOSXR.SeaShark.Tests
{
    [TestFixture]
    public class VectorMathTests
    {
        private const float Tolerance = 0.0001f;

        // ── GetDotProduct ─────────────────────────────────────────────────────

        [Test]
        public void GetDotProduct_ParallelVectors_ReturnsPositive()
        {
            var dot = VectorMath.GetDotProduct(Vector3.forward, Vector3.forward);
            Assert.AreEqual(1f, dot, Tolerance);
        }

        [Test]
        public void GetDotProduct_OppositeVectors_ReturnsNegative()
        {
            var dot = VectorMath.GetDotProduct(Vector3.forward, Vector3.back);
            Assert.AreEqual(-1f, dot, Tolerance);
        }

        [Test]
        public void GetDotProduct_PerpendicularVectors_ReturnsZero()
        {
            var dot = VectorMath.GetDotProduct(Vector3.forward, Vector3.right);
            Assert.AreEqual(0f, dot, Tolerance);
        }

        // ── RemoveDotVector ───────────────────────────────────────────────────

        [Test]
        public void RemoveDotVector_RemovesDirectionComponent()
        {
            // vector = (1,1,0), direction = (1,0,0) → removes x component → (0,1,0)
            var result = VectorMath.RemoveDotVector(new Vector3(1f, 1f, 0f), Vector3.right);
            Assert.AreEqual(0f, result.x, Tolerance);
            Assert.AreEqual(1f, result.y, Tolerance);
            Assert.AreEqual(0f, result.z, Tolerance);
        }

        [Test]
        public void RemoveDotVector_PerpendicularDirection_LeavesVectorUnchanged()
        {
            // Forward vector, remove upward component — no upward component to remove
            var result = VectorMath.RemoveDotVector(Vector3.forward, Vector3.up);
            Assert.AreEqual(0f, result.x, Tolerance);
            Assert.AreEqual(0f, result.y, Tolerance);
            Assert.AreEqual(1f, result.z, Tolerance);
        }

        // ── ExtractDotVector ──────────────────────────────────────────────────

        [Test]
        public void ExtractDotVector_ExtractsDirectionComponent()
        {
            // vector = (3,4,0), direction = (1,0,0) → extracts x → (3,0,0)
            var result = VectorMath.ExtractDotVector(new Vector3(3f, 4f, 0f), Vector3.right);
            Assert.AreEqual(3f, result.x, Tolerance);
            Assert.AreEqual(0f, result.y, Tolerance);
            Assert.AreEqual(0f, result.z, Tolerance);
        }

        [Test]
        public void RemoveAndExtract_SumToOriginal()
        {
            var original = new Vector3(2f, 5f, 0f);
            var direction = Vector3.up;
            var removed = VectorMath.RemoveDotVector(original, direction);
            var extracted = VectorMath.ExtractDotVector(original, direction);
            var sum = removed + extracted;
            Assert.AreEqual(original.x, sum.x, Tolerance);
            Assert.AreEqual(original.y, sum.y, Tolerance);
            Assert.AreEqual(original.z, sum.z, Tolerance);
        }

        // ── ProjectPointOntoLine ──────────────────────────────────────────────

        [Test]
        public void ProjectPointOntoLine_PointOnLine_ReturnsSamePoint()
        {
            var start = Vector3.zero;
            var direction = Vector3.right;
            var point = new Vector3(5f, 0f, 0f);
            var result = VectorMath.ProjectPointOntoLine(start, direction, point);
            Assert.AreEqual(5f, result.x, Tolerance);
            Assert.AreEqual(0f, result.y, Tolerance);
        }

        [Test]
        public void ProjectPointOntoLine_PointAboveLine_ProjectsOntoLine()
        {
            var start = Vector3.zero;
            var direction = Vector3.right;
            var point = new Vector3(3f, 4f, 0f); // above the X-axis line
            var result = VectorMath.ProjectPointOntoLine(start, direction, point);
            Assert.AreEqual(3f, result.x, Tolerance);
            Assert.AreEqual(0f, result.y, Tolerance);
        }

        // ── IncrementVectorTowardTargetVector ─────────────────────────────────

        [Test]
        public void IncrementVectorTowardTargetVector_MovesCloser()
        {
            var current = Vector3.zero;
            var target = new Vector3(10f, 0f, 0f);
            var result = VectorMath.IncrementVectorTowardTargetVector(current, 5f, 1f, target);
            Assert.AreEqual(5f, result.x, Tolerance);
        }

        [Test]
        public void IncrementVectorTowardTargetVector_DoesNotOvershoot()
        {
            var current = Vector3.zero;
            var target = new Vector3(1f, 0f, 0f);
            var result = VectorMath.IncrementVectorTowardTargetVector(current, 100f, 1f, target);
            Assert.AreEqual(target.x, result.x, Tolerance);
        }

        // ── GetAngle ──────────────────────────────────────────────────────────

        [Test]
        public void GetAngle_SameVectors_ReturnsZero()
        {
            var angle = VectorMath.GetAngle(Vector3.forward, Vector3.forward, Vector3.up);
            Assert.AreEqual(0f, angle, Tolerance);
        }

        [Test]
        public void GetAngle_PerpendicularVectorsOnPlane_Returns90Degrees()
        {
            // Cross(forward, right) = up; Dot(up, up) = 1; Sign = +1 → angle = +90
            var angle = VectorMath.GetAngle(Vector3.forward, Vector3.right, Vector3.up);
            Assert.AreEqual(90f, angle, Tolerance);
        }
    }
}
