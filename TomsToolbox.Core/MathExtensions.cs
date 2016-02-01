namespace TomsToolbox.Core
{
    using System;

    /// <summary>
    /// Extension methods for math operations.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Clips the specified value so it does not exceed min or max.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The clipped value.</returns>
        public static double Clip(this double value, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        /// <summary>
        /// Clips the specified value so it does not exceed min or max.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The clipped value.</returns>
        public static float Clip(this float value, float minValue, float maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        /// <summary>
        /// Clips the specified value so it does not exceed min or max.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The clipped value.</returns>
        public static int Clip(this int value, int minValue, int maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }

        /// <summary>
        /// Clips the specified value so it does not exceed min or max.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>The clipped value.</returns>
        public static long Clip(this long value, long minValue, long maxValue)
        {
            return Math.Min(Math.Max(value, minValue), maxValue);
        }
    }
}
