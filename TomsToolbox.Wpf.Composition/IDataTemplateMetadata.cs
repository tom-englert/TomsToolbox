namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Windows;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;

    /// <summary>
    /// Meta data for exported visuals.
    /// </summary>
    public interface IDataTemplateMetadata
    {
        /// <summary>
        /// Gets the type for which this DataTemplate is intended.
        /// </summary>
        [CanBeNull]
        Type DataType
        {
            get;
        }

        /// <summary>
        /// Gets the role of this visual.
        /// If a role is set, a <see cref="RoleBasedDataTemplateKey"/> will be created for this view; else a simple <see cref="DataTemplateKey"/> is used.
        /// </summary>
        [CanBeNull]
        object Role
        {
            get;
        }
    }

    internal class DataTemplateMetadata : IDataTemplateMetadata
    {
        public DataTemplateMetadata([CanBeNull] IMetadata  metadata)
        {
            if (metadata == null)
                return;

            if (metadata.TryGetValue(nameof(DataType), out var viewModel))
            {
                DataType = viewModel as Type;
            }

            if (metadata.TryGetValue(nameof(Role), out var role))
            {
                Role = role;
            }
        }

        [CanBeNull]
        public Type DataType { get; }
        
        [CanBeNull]
        public object Role { get; }
    }
}
