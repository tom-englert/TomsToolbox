namespace TomsToolbox.Wpf.Styles
{
    using System.Windows;
    using System.Windows.Interactivity;

    using TomsToolbox.Essentials;

    /// <summary>
    /// A behavior to call all external theme resource providers. Theme resource providers must implement and export <see cref="IThemeResourceProvider"/>.
    /// </summary>
    public class ThemeResourceLoaderBehavior : Behavior<Window>
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

            var window = AssociatedObject;

            Dispatcher?.BeginInvoke(() =>
            {
                window?.TryGetExportProvider()?
                    .GetExportedValues<IThemeResourceProvider>()
                    .ForEach(resourceProvider => resourceProvider?.LoadThemeResources(window.Resources));
            });
        }
    }
}
