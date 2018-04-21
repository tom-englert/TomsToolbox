namespace TomsToolbox.Wpf.Composition
{
    using System;
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
        [NotNull]
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
}
