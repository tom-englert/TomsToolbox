namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// Meta data for exported visuals.
    /// </summary>
    public interface IDataTemplateMetadata
    {
        /// <summary>
        /// Gets the type of the view model that this visual has a representation for.
        /// </summary>
        [CanBeNull]
        Type ViewModel
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
        public DataTemplateMetadata([CanBeNull] IDictionary<string, object>  metadata)
        {
            if (metadata == null)
                return;

            if (metadata.TryGetValue(nameof(ViewModel), out var viewModel))
            {
                ViewModel = viewModel as Type;
            }

            if (metadata.TryGetValue(nameof(Role), out var role))
            {
                Role = role;
            }
        }

        [CanBeNull]
        public Type ViewModel { get; }
        
        [CanBeNull]
        public object Role { get; }
    }
}
