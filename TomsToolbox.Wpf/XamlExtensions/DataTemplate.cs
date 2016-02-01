namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;

    using TomsToolbox.Core;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Attached properties to simplify data template handling.
    /// </summary>
    public static class DataTemplate
    {
        /// <summary>
        /// Gets the role associated with a data template.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The role.</returns>
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static object GetRole(Control obj)
        {
            Contract.Requires(obj != null);
            return obj.GetValue(RoleProperty);
        }
        /// <summary>
        /// Sets the role associated with a data template.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRole(Control obj, object value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(RoleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.XamlExtensions.DataTemplate.Role"/> dependency property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>Shortcut to set a <see cref="RoleBasedDataTemplateSelector"/> with the specified role as the targets <see cref="ContentControl.ContentTemplateSelector"/>.</summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(object), typeof(DataTemplate), new FrameworkPropertyMetadata(Role_Changed));

        private static void Role_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;

            d.TryCast()
                .When<ContentControl>(cc => cc.ContentTemplateSelector = new RoleBasedDataTemplateSelector {Role = newValue})
                .When<TabControl>(tc => tc.ContentTemplateSelector = new RoleBasedDataTemplateSelector {Role = newValue});
        }
    }
}
