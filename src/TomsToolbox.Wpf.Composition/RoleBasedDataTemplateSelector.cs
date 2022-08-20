namespace TomsToolbox.Wpf.Composition;

using System.Linq;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// A template selector that finds the <see cref="DataTemplate"/> by the <see cref="RoleBasedDataTemplateKey"/>.
/// </summary>
public class RoleBasedDataTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the role to use in the lookup.
    /// </summary>
    public object? Role
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the template used as fallback if no template for the specified role is found.
    /// </summary>
    public DataTemplate? FallbackValue
    {
        get;
        set;
    }

    /// <summary>
    /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate" /> based on custom logic.
    /// </summary>
    /// <param name="item">The data object for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>
    /// Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.
    /// </returns>
    public override DataTemplate? SelectTemplate(object? item, DependencyObject? container)
    {
        if ((item == null) || (container == null))
            return null;

        var frameworkElement = container.AncestorsAndSelf().OfType<FrameworkElement>().FirstOrDefault();
        if (frameworkElement == null)
            return null;

        var key = DataTemplateManager.CreateKey(item.GetType(), Role);

        return (frameworkElement.TryFindResource(key) as DataTemplate) ?? FallbackValue;
    }
}