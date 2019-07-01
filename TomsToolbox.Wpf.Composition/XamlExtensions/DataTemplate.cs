namespace TomsToolbox.Wpf.XamlExtensions
{
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

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
        [CanBeNull]
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        [AttachedPropertyBrowsableForType(typeof(ContentPresenter))]
        public static object GetRole([NotNull] FrameworkElement obj)
        {
            return obj.GetValue(RoleProperty);
        }
        /// <summary>
        /// Sets the role associated with a data template.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRole([NotNull] FrameworkElement obj, [CanBeNull] object value)
        {
            obj.SetValue(RoleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.XamlExtensions.DataTemplate.Role"/> dependency property.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>Shortcut to set a <see cref="RoleBasedDataTemplateSelector"/> with the specified role as the targets <see cref="ContentControl.ContentTemplateSelector"/>.</summary>
        /// </AttachedPropertyComments>
        [NotNull]
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.RegisterAttached("Role", typeof(object), typeof(DataTemplate), new FrameworkPropertyMetadata(Role_Changed));

        private static void Role_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
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
