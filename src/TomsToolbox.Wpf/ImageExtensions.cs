namespace TomsToolbox.Wpf;

using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Extension for the <see cref="Image"/> class:
/// </summary>
public static class ImageExtensions
{
    /// <summary>
    /// Gets the resource key from which to load the image source.
    /// </summary>
    /// <param name="obj">The image.</param>
    /// <returns>The resource key.</returns>
    [AttachedPropertyBrowsableForType(typeof(Image))]
    public static object? GetResourceKey(this Image obj)
    {
        return obj.GetValue(ResourceKeyProperty);
    }
    /// <summary>
    /// Sets the resource key from which to load the image source.
    /// </summary>
    /// <param name="obj">The image.</param>
    /// <param name="value">The resource key.</param>
    [AttachedPropertyBrowsableForType(typeof(Image))]
    public static void SetResourceKey(this Image obj, object? value)
    {
        obj.SetValue(ResourceKeyProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wpf.ImageExtensions.ResourceKey"/> dependency property.
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>Allows to specify a resource key instead of an Uri as the source from which the image will be loaded.</summary>
    /// </AttachedPropertyComments>
    public static readonly DependencyProperty ResourceKeyProperty =
        DependencyProperty.RegisterAttached("ResourceKey", typeof(object), typeof(ImageExtensions), new FrameworkPropertyMetadata((sender, e) => ResourceKey_Changed((Image)sender, e.NewValue)));

    private static void ResourceKey_Changed(Image image, object? resourceKey)
    {
        image.Source = (resourceKey != null) ? image.TryFindResource(resourceKey) as ImageSource : null;
        image.ImageFailed -= Image_ImageFailed;
        image.ImageFailed += Image_ImageFailed;
    }

    static void Image_ImageFailed(object? sender, ExceptionRoutedEventArgs e)
    {
        if (!(sender is Image image))
            return;
        var resourceKey = GetResourceKey(image);
        var message = e.ErrorException?.Message ?? @"No exception";
        Trace.TraceError(string.Format(CultureInfo.InvariantCulture, "Load image with resource key '{0}' failed: {1}", resourceKey, message));
    }
}