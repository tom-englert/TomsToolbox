namespace TomsToolbox.Wpf.Composition.AttributedModel
{
    using System;
    using System.Composition;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition.XamlExtensions;

    /// <summary>
    /// Attribute to apply to view models to support visual composition.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VisualCompositionExportAttribute : ExportAttribute, IVisualCompositionMetadata
    {
        [NotNull, ItemNotNull]
        private readonly string[] _targetRegions;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCompositionExportAttribute" /> class.
        /// </summary>
        /// <param name="targetRegions">The names of the region(s) where this view should appear.</param>
        public VisualCompositionExportAttribute([NotNull, ItemNotNull] params string[] targetRegions)
            : base(VisualComposition.ExportContractName, typeof(object))
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
