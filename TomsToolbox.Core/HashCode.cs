namespace TomsToolbox.Core
{
    /// <summary>
    /// Helper to generate hash codes
    /// </summary>
    public static class HashCode
    {
        /// <summary>
        /// Aggregates the specified values to a single hash code.
        /// </summary>
        /// <param name="h1">The first hash code.</param>
        /// <param name="h2">The second hash code.</param>
        /// <returns>A new hash code calculated from the specified ones.</returns>
        public static int Aggregate(int h1, int h2)
        {
            unchecked
            {
                return ((h1 << 5) + h1) ^ h2;
            }
        }
    }
}
