namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition.XamlExtensions;

    /// <summary>
    /// A control used to create dynamic content from an IOC container.
    /// </summary>
    public class ComposableContentControl : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComposableContentControl"/> class.
        /// </summary>
        public ComposableContentControl()
        {
            Focusable = false;
        }


        /// <summary>
        /// Gets or sets the role of the template.
        /// </summary>
        [CanBeNull]
        public object Role
        {
            get => GetValue(RoleProperty);
            set => SetValue(RoleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Role"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(object), typeof(ComposableContentControl));


        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized" /> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized" /> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            Update();
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ((e.Property == DataContextProperty)
                || ((e.Property == ExportProviderLocator.ExportProviderProperty) && (e.NewValue != null))
                || (e.Property == RoleProperty))
            {
                Update();
            }
        }

        private void Update()
        {
            try
            {
                if (!IsInitialized)
                    return;

                Content = null;

                var dataContext = DataContext;

                if (dataContext == null)
                    return;

                var exportProvider = this.GetExportProvider();

                var viewModel = dataContext.GetType();
                var view = exportProvider.GetDataTemplateView(viewModel, Role);

                if (view == null)
                    return;

                DataTemplateManager.SetRole(view, Role);
                Content = view;
            }
            catch (Exception ex)
            {
                Content = new TextBox { Text = ex.ToString(), IsReadOnly = true };

                VisualComposition.OnError(this, ex.ToString());
            }
        }
    }
}
