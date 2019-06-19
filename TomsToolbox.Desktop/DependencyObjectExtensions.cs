namespace TomsToolbox.Desktop
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Extension methods for the <see cref="DependencyObject"/>.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Gets the value of a dependency property using <see cref="ObjectExtensions.SafeCast{T}(object)" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The dependency object from which to get the value.</param>
        /// <param name="property">The property to get.</param>
        /// <returns>The value safely casted to <typeparamref name="T"/></returns>
        [CanBeNull]
        public static T GetValue<T>([NotNull] this DependencyObject self, [NotNull] DependencyProperty property)
        {
            return self.GetValue(property).SafeCast<T>();
        }

        /// <summary>
        /// Tracks the changes of the specified property.
        /// </summary>
        /// <typeparam name="T">The type of the dependency object to track.</typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="property">The property to track.</param>
        /// <returns>The object providing the changed event.</returns>
        [NotNull]
        public static INotifyChanged Track<T>([NotNull] this T dependencyObject, [NotNull] DependencyProperty property)
            where T : DependencyObject
        {
            return new DependencyPropertyEventWrapper<T>(dependencyObject, property);
        }

        private class DependencyPropertyEventWrapper<T> : INotifyChanged
            where T : DependencyObject
        {
            [NotNull]
            private readonly T _dependencyObject;
            [CanBeNull]
            private readonly DependencyPropertyDescriptor _dependencyPropertyDescriptor;

            public DependencyPropertyEventWrapper([NotNull] T dependencyObject, [NotNull] DependencyProperty property)
            {
                _dependencyObject = dependencyObject;
                _dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            }

            public event EventHandler Changed
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                add => _dependencyPropertyDescriptor?.AddValueChanged(_dependencyObject, value);
                // ReSharper disable once AssignNullToNotNullAttribute
                remove => _dependencyPropertyDescriptor?.RemoveValueChanged(_dependencyObject, value);
            }
        }
    }
}
