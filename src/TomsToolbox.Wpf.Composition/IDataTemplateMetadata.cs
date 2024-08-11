namespace TomsToolbox.Wpf.Composition;

using System;
using System.Windows;

/// <summary>
/// Meta data for exported visuals.
/// </summary>
public interface IDataTemplateMetadata
{
    /// <summary>
    /// Gets the type for which this DataTemplate is intended.
    /// </summary>
    Type? DataType
    {
        get;
    }

    /// <summary>
    /// Gets the role of this visual.
    /// If a role is set, a <see cref="RoleBasedDataTemplateKey"/> will be created for this view; else a simple <see cref="DataTemplateKey"/> is used.
    /// </summary>
    object? Role
    {
        get;
    }
}
