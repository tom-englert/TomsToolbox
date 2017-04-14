namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// Provides location service for the export provider to the WPF UI tree.
    /// </summary>
    public static class ExportProviderLocator
    {
        /// <summary>
        /// Registers the specified export provider.
        /// </summary>
        /// <param name="exportProvider">The export provider.</param>
        public static void Register([NotNull] ExportProvider exportProvider)
        {
            Contract.Requires(exportProvider != null);
            ExportProviderProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(exportProvider, FrameworkPropertyMetadataOptions.Inherits));
        }

        /// <summary>
        /// Gets the active export provider for the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The exports provider.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Export provider must be registered in the visual tree</exception>
        [NotNull]
        public static ExportProvider GetExportProvider([NotNull] this DependencyObject obj)
        {
            Contract.Requires(obj != null);
            Contract.Ensures(Contract.Result<ExportProvider>() != null);

            var exportProvider = (ExportProvider)obj.GetValue(ExportProviderProperty);
            if (exportProvider == null)
                throw new InvalidOperationException(GetMissingExportProviderMessage(obj));

            return exportProvider;
        }
        /// <summary>
        /// Gets the active export provider for the specified object, or <c>null</c> if no export provider is registered.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The exports provider.
        /// </returns>
        public static ExportProvider TryGetExportProvider([NotNull] this DependencyObject obj)
        {
            Contract.Requires(obj != null);

            return (ExportProvider)obj.GetValue(ExportProviderProperty);
        }
        /// <summary>
        /// Sets the export provider.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetExportProvider([NotNull] this DependencyObject obj, ExportProvider value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(ExportProviderProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.Composition.ExportProviderLocator.ExportProvider"/> attached property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Makes the export provider available in the WPF visual tree.
        /// </summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ExportProviderProperty =
            DependencyProperty.RegisterAttached("ExportProvider", typeof(ExportProvider), typeof(ExportProviderLocator), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the message to show when an export provider could not be located in the visual tree.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The message.</returns>
        [NotNull]
        internal static string GetMissingExportProviderMessage([NotNull] this DependencyObject obj)
        {
            Contract.Requires(obj != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return "Export provider must be registered in the visual tree " + string.Join("/", obj.AncestorsAndSelf().Reverse().Select(o => o?.GetType().Name));
        }
    }
}
