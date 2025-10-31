using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From Timeline 1.8.8 examples
    /// </summary>
    public static class QuaternionUtils
    {
        private const float _floatMin = 1e-10f;

        public static readonly Quaternion Zero = new(0f, 0f, 0f, 0f);


        public static Quaternion Scale(this Quaternion q, float scale)
        {
            return new Quaternion(q.x * scale, q.y * scale, q.z * scale, q.w * scale);
        }


        public static Quaternion NormalizeSafe(this Quaternion q)
        {
            var dot = Quaternion.Dot(q, q);

            if (dot > _floatMin)
            {
                var rsqrt = 1.0f / Mathf.Sqrt(dot);

                return new Quaternion(q.x * rsqrt, q.y * rsqrt, q.z * rsqrt, q.w * rsqrt);
            }

            return Quaternion.identity;
        }


        public static Quaternion Blend(this Quaternion q1, Quaternion q2, float weight)
        {
            return q1.Add(q2.Scale(weight));
        }


        public static Quaternion Add(this Quaternion rhs, Quaternion lhs)
        {
            var sign = Mathf.Sign(Quaternion.Dot(rhs, lhs));

            return new Quaternion(rhs.x + sign * lhs.x, rhs.y + sign * lhs.y, rhs.z + sign * lhs.z, rhs.w + sign * lhs.w);
        }
    }
}