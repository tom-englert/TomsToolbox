namespace TomsToolbox.Wpf.Composition
{
    using JetBrains.Annotations;

    using TomsToolbox.Composition;

    /// <summary>
    /// Export metadata for composable objects.
    /// </summary>
    public interface IVisualCompositionMetadata
    {
        /// <summary>
        /// Gets the id of the item for visual composition.
        /// </summary>
        [CanBeNull]
        object? Role
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
        [CanBeNull, ItemCanBeNull]
        string?[]? TargetRegions
        {
            get;
        }
    }

    internal class VisualCompositionMetadata : IVisualCompositionMetadata
    {
        public VisualCompositionMetadata([CanBeNull] IMetadata? metadata)
        {
            if (metadata == null)
                return;

            if (metadata.TryGetValue(nameof(Role), out var role))
            {
                Role = role;
            }

            if (metadata.TryGetValue(nameof(Sequence), out var sequence) && (sequence is double d))
            {
                Sequence = d;
            }

            if (metadata.TryGetValue(nameof(TargetRegions), out var targetRegions))
            {
                TargetRegions = targetRegions as string[];
            }
        }

        [CanBeNull]
        public object? Role { get; }

        public double Sequence { get; }

        [CanBeNull, ItemCanBeNull]
        public string?[]? TargetRegions { get; }
    }
}
