namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.ComponentModel.Composition;

    using JetBrains.Annotations;

    /// <summary>
    /// Attribute to apply to view models to support visual composition.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    [CLSCompliant(false)] // attributes must used arrays, even as properties.
    public sealed class VisualCompositionExportAttribute : ExportAttribute, IVisualCompositionMetadata
    {
        /// <summary>
        /// The contract name for visual composition exports.
        /// </summary>
        public const string ExportContractName = "VisualComposition-86E8D1EF-1322-46B4-905C-115AAD63533D";

        [NotNull, ItemNotNull]
        private readonly string[] _targetRegions;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCompositionExportAttribute" /> class.
        /// </summary>
        /// <param name="targetRegions">The names of the region(s) where this view should appear.</param>
        public VisualCompositionExportAttribute([NotNull, ItemNotNull] params string[] targetRegions)
            : base(ExportContractName, typeof(object))
        {
            _targetRegions = targetRegions;
        }

        /// <summary>
        /// Gets the role of the view model for visual composition.
        /// </summary>
        public object Role
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a sequence to support ordering of view model collections.
        /// </summary>
        public double Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the target regions for visual composition.
        /// </summary>
        [NotNull]
        public string[] TargetRegions
        {
            get
            {
                return _targetRegions;
            }
        }
    }
}
