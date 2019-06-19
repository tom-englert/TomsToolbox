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
        /// <param name="hash1">The first hash code.</param>
        /// <param name="hash2">The second hash code.</param>
        /// <returns>A new hash code calculated from the specified ones.</returns>
        public static int Aggregate(int hash1, int hash2)
        {
            unchecked
            {
                return ((hash1 << 5) + hash1) ^ hash2;
            }
        }
    }
}
