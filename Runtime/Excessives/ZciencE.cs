using System;


namespace SOSXR.SeaShark.Excessives
{
    /// <summary>
    /// Science and physics utility functions, primarily for special relativity calculations.
    /// Provides methods for calculating relativistic effects like length contraction, mass dilation, and time dilation.
    /// </summary>
    public static class ZciencE
    {
        #region Special Relativity

        /// <summary>
        /// Calculates the Lorentz factor (gamma) used in relativistic calculations.
        /// Gamma = sqrt(1 - v²/c²), where v is velocity and c is the speed of light.
        /// </summary>
        /// <param name="velocity">The velocity in the same units as speedOfLight.</param>
        /// <param name="speedOfLight">The speed of light (default: 3 * 10^8 m/s).</param>
        /// <returns>The Lorentz factor (gamma).</returns>
        public static double GammaMod(double velocity, double speedOfLight = 3 * (10 ^ 8))
        {
            return Math.Sqrt(1 - velocity * velocity / (speedOfLight * speedOfLight));
        }


        /// <summary>
        /// Calculates the length contraction of an object moving at relativistic speeds.
        /// Length contraction = originalLength * gamma, where gamma is the Lorentz factor.
        /// </summary>
        /// <param name="originalLength">The rest length of the object.</param>
        /// <param name="velocity">The velocity of the object in the same units as speedOfLight.</param>
        /// <param name="speedOfLight">The speed of light (default: 3 * 10^8 m/s).</param>
        /// <returns>The contracted length as observed from a stationary frame.</returns>
        public static double LengthContraction(
            double originalLength,
            double velocity,
            double speedOfLight = 3 * (10 ^ 8))
        {
            return originalLength * GammaMod(velocity, speedOfLight);
        }


        /// <summary>
        /// Calculates the relativistic mass increase of an object moving at relativistic speeds.
        /// Relativistic mass = originalMass / gamma, where gamma is the Lorentz factor.
        /// </summary>
        /// <param name="originalMass">The rest mass of the object.</param>
        /// <param name="velocity">The velocity of the object in the same units as speedOfLight.</param>
        /// <param name="speedOfLight">The speed of light (default: 3 * 10^8 m/s).</param>
        /// <returns>The relativistic mass as observed from a stationary frame.</returns>
        public static double MassDilation(
            double originalMass,
            double velocity,
            double speedOfLight = 3 * (10 ^ 8))
        {
            return originalMass / GammaMod(velocity, speedOfLight);
        }


        /// <summary>
        /// Calculates the time dilation effect for an object moving at relativistic speeds.
        /// Dilated time = originalTime / gamma, where gamma is the Lorentz factor.
        /// </summary>
        /// <param name="originalTime">The time interval in the moving object's reference frame.</param>
        /// <param name="velocity">The velocity of the object in the same units as speedOfLight.</param>
        /// <param name="speedOfLight">The speed of light (default: 3 * 10^8 m/s).</param>
        /// <returns>The time interval as observed from a stationary frame.</returns>
        public static double TimeDilation(
            double originalTime,
            double velocity,
            double speedOfLight = 3 * (10 ^ 8))
        {
            return originalTime / GammaMod(velocity, speedOfLight);
        }

        #endregion
    }
}