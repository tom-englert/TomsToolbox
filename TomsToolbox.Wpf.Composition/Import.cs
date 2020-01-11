namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Linq;
    using System.Windows;

    using JetBrains.Annotations;

    using Microsoft.Xaml.Behaviors;

    /// <summary>
    /// A helper class to host attached properties for import.
    /// </summary>
    public static class Import
    {
        /// <summary>
        /// Gets the data context type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The data context type.</returns>
        [CanBeNull]
        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static Type GetDataContext([NotNull] FrameworkElement obj)
        {
            return (Type)obj.GetValue(DataContextProperty);
        }
        /// <summary>
        /// Sets the data context type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetDataContext([NotNull] FrameworkElement obj, [CanBeNull] Type? value)
        {
            obj.SetValue(DataContextProperty, value);
        }
        /// <summary>
        /// Identifies the <see>TomsToolbox.Wpf.Composition.Import.DataContext</see> dependency property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Attach this property to inject a <see cref="ImportBehavior"/> with this type as the target for the data context into the attached object.
        /// </summary>
        /// </AttachedPropertyComments>
        [NotNull] public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached("DataContext", typeof(Type), typeof(Import), new FrameworkPropertyMetadata(null, DataContext_Changed));


        private static void DataContext_Changed([NotNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behaviors = Interaction.GetBehaviors(d);

            var behavior = behaviors.OfType<ImportBehavior>().FirstOrDefault();
            if (behavior == null)
            {
                behaviors.Add(new ImportBehavior { MemberType = (Type)e.NewValue });
            }
            else
            {
                behavior.MemberType = (Type)e.NewValue;
            }
        }
    }
}