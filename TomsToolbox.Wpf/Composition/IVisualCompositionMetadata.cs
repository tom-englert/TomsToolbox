namespace TomsToolbox.Wpf.Composition
{
    /// <summary>
    /// Export metadata for <see cref="IComposablePart"/> objects.
    /// </summary>
    public interface IVisualCompositionMetadata
    {
        /// <summary>
        /// Gets the id of the item for visual composition.
        /// </summary>
        string Role
        {
            get;
        }

        /// <summary>
        /// Gets a sequence to provide ordering of lists.
        /// </summary>
        double Sequence
        {
            get;
        }

        /// <summary>
        /// Gets the target regions for visual composition.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Export metadata requires array.")]
        string[] TargetRegions
        {
            get;
        }
    }
}
