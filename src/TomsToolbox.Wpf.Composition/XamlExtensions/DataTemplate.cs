namespace TomsToolbox.Wpf.Composition.XamlExtensions
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Attached properties to simplify data template handling.
    /// </summary>
    public static class DataTemplate
    {
        /// <summary>
        /// The contract name used for export.
        /// </summary>
        public const string ContractName = "{41cf1dfc-9c56-4e06-b177-703b4a24b0e1}";

        /// <summary>
        /// Gets the role associated with a data template.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The role.</returns>
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(ContentPresenter))]
        public static object? GetRole(FrameworkElement obj)
        {
            return obj.GetValue(RoleProperty);
        }
        /// <summary>
        /// Sets the role associated with a data template.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRole(FrameworkElement obj, object? value)
        {
            obj.SetValue(RoleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.Composition.XamlExtensions.DataTemplate.Role"/> dependency property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>Shortcut to set a <see cref="RoleBasedDataTemplateSelector"/> with the specified role as the targets <see cref="ContentControl.ContentTemplateSelector"/>.</summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(object), typeof(DataTemplate), new FrameworkPropertyMetadata(Role_Changed));

        private static void Role_Changed(DependencyObject? d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;

            switch (d)
            {
                case ContentControl cc:
                    cc.ContentTemplateSelector = new RoleBasedDataTemplateSelector { Role = newValue };
                    break;
                case TabControl tc:
                    tc.ContentTemplateSelector = new RoleBasedDataTemplateSelector { Role = newValue };
                    break;
                case ContentPresenter cp: 
                    cp.ContentTemplateSelector = new RoleBasedDataTemplateSelector { Role = newValue };
                    break;
            }
        }
    }
}
