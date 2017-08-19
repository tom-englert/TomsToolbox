namespace TomsToolbox.Wpf.Composition
{
    using System.Diagnostics.CodeAnalysis;

    using JetBrains.Annotations;

    /// <summary>
    /// Export metadata for composable objects.
    /// </summary>
    public interface IVisualCompositionMetadata
    {
        /// <summary>
        /// Gets the id of the item for visual composition.
        /// </summary>
        [CanBeNull]
        object Role
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
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Export metadata requires array.")]
        [CanBeNull, ItemCanBeNull]
        string[] TargetRegions
        {
            get;
        }
    }
}
