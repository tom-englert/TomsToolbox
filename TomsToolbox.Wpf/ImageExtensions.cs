namespace TomsToolbox.Wpf
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using JetBrains.Annotations;

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
        [CanBeNull]
        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static object GetResourceKey([NotNull] this Image obj)
        {
            return obj.GetValue(ResourceKeyProperty);
        }
        /// <summary>
        /// Sets the resource key from which to load the image source.
        /// </summary>
        /// <param name="obj">The image.</param>
        /// <param name="value">The resource key.</param>
        [AttachedPropertyBrowsableForType(typeof(Image))]
        public static void SetResourceKey([NotNull] this Image obj, [CanBeNull] object value)
        {
            obj.SetValue(ResourceKeyProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.ImageExtensions.ResourceKey"/> dependency property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>Allows to specify a resource key instead of an Uri as the source from which the image will be loaded.</summary>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty ResourceKeyProperty =
            // ReSharper disable once AssignNullToNotNullAttribute
            DependencyProperty.RegisterAttached("ResourceKey", typeof(object), typeof(ImageExtensions), new FrameworkPropertyMetadata((sender, e) => ResourceKey_Changed((Image)sender, e.NewValue)));

        private static void ResourceKey_Changed([NotNull] Image image, [CanBeNull] object resourceKey)
        {
            image.Source = (resourceKey != null) ? image.TryFindResource(resourceKey) as ImageSource : null;
            image.ImageFailed -= Image_ImageFailed;
            image.ImageFailed += Image_ImageFailed;
        }

        static void Image_ImageFailed([NotNull] object sender, [NotNull] ExceptionRoutedEventArgs e)
        {
            var resourceKey = GetResourceKey((Image)sender);
            var message = e.ErrorException?.Message ?? @"No exception";
            Trace.TraceError(string.Format(CultureInfo.InvariantCulture, "Load image with resource key '{0}' failed: {1}", resourceKey, message));
        }
    }
}
