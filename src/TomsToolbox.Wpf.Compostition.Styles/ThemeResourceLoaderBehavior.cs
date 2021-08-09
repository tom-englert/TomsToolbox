namespace TomsToolbox.Wpf.Composition.Styles
{
    using System.Windows;

    using Microsoft.Xaml.Behaviors;

    using TomsToolbox.Essentials;

    /// <summary>
    /// A behavior to call all external theme resource providers. Theme resource providers must implement and export <see cref="IThemeResourceProvider"/>.
    /// </summary>
    public class ThemeResourceLoaderBehavior : Behavior<DependencyObject>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            var window = AssociatedObject as Window ?? Window.GetWindow(AssociatedObject);

            Dispatcher?.BeginInvoke(() =>
            {
                window?.TryGetExportProvider()?
                    .GetExportedValues<IThemeResourceProvider>()
                    .ForEach(resourceProvider => resourceProvider.LoadThemeResources(window.Resources));
            });
        }
    }
}
