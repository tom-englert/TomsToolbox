namespace TomsToolbox.Wpf.Composition.Styles;

using System.Windows;

/// <summary>
/// The interface to be implemented and exported by application specific theme resource providers. 
/// Theme resource providers can load additional theme specific resources into the resource dictionary.
/// </summary>
public interface IThemeResourceProvider
{
    /// <summary>
    /// Called by the styles to loads the theme specific resources.
    /// </summary>
    /// <param name="resource">The resource where the provider can add the theme specific resource.</param>
    void LoadThemeResources(ResourceDictionary resource);
}