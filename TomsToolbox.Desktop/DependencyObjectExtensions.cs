namespace TomsToolbox.Desktop
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
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
            Contract.Requires(self != null);
            Contract.Requires(property != null);

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
            Contract.Requires(dependencyObject != null);
            Contract.Requires(property != null);
            Contract.Ensures(Contract.Result<INotifyChanged>() != null);

            return new DependencyPropertyEventWrapper<T>(dependencyObject, property);
        }


        class DependencyPropertyEventWrapper<T> : INotifyChanged
            where T : DependencyObject
        {
            [NotNull]
            private readonly T _dependencyObject;
            private readonly DependencyPropertyDescriptor _dependencyPropertyDescriptor;

            public DependencyPropertyEventWrapper([NotNull] T dependencyObject, [NotNull] DependencyProperty property)
            {
                Contract.Requires(dependencyObject != null);
                Contract.Requires(property != null);

                _dependencyObject = dependencyObject;
                _dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(property, typeof(T));
            }

            public event EventHandler Changed
            {
                add
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _dependencyPropertyDescriptor?.AddValueChanged(_dependencyObject, value);
                }
                remove
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _dependencyPropertyDescriptor?.RemoveValueChanged(_dependencyObject, value);
                }
            }

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            [Conditional("CONTRACTS_FULL")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_dependencyObject != null);
            }
        }
    }
}
