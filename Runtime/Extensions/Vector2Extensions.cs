using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Extension methods for UnityEngine.Vector2.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        ///     Adds to the X and/or Y components of a Vector2.
        /// </summary>
        /// <param name="vector2">The original vector.</param>
        /// <param name="x">Amount to add to the X component (default 0).</param>
        /// <param name="y">Amount to add to the Y component (default 0).</param>
        /// <returns>A new Vector2 with updated components.</returns>
        public static Vector2 Add(this Vector2 vector2, float x = 0, float y = 0)
        {
            return new Vector2(vector2.x + x, vector2.y + y);
        }


        /// <summary>
        ///     Sets the X and/or Y components of a Vector2.
        /// </summary>
        /// <param name="vector2">The original vector.</param>
        /// <param name="x">New X value (nullable).</param>
        /// <param name="y">New Y value (nullable).</param>
        /// <returns>A new Vector2 with the specified components set.</returns>
        public static Vector2 With(this Vector2 vector2, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector2.x, y ?? vector2.y);
        }


        /// <summary>
        ///     Returns a Boolean indicating whether the current Vector2 is in a given range from another Vector2
        /// </summary>
        /// <param name="current">The current Vector2 position</param>
        /// <param name="target">The Vector2 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current Vector2 is in the given range from the target Vector2, false otherwise</returns>
        public static bool InRangeOf(this Vector2 current, Vector2 target, float range)
        {
            return (current - target).sqrMagnitude <= range * range;
        }


        /// <summary>
        ///     Computes a random point in an annulus (a ring-shaped area) based on minimum and
        ///     maximum radius values around a central Vector2 point (origin).
        /// </summary>
        /// <param name="origin">The center Vector2 point of the annulus.</param>
        /// <param name="minRadius">Minimum radius of the annulus.</param>
        /// <param name="maxRadius">Maximum radius of the annulus.</param>
        /// <returns>A random Vector2 point within the specified annulus.</returns>
        public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius)
        {
            var angle = Random.value * Mathf.PI * 2f;
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            var minRadiusSquared = minRadius * minRadius;
            var maxRadiusSquared = maxRadius * maxRadius;
            var distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);

            // Calculate the position vector
            var position = direction * distance;

            return origin + position;
        }
    }
}
