[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "TomsToolbox.Wpf.Composition.ComposablePartWithContext`1.#TomsToolbox.Wpf.Composition.IComposablePartWithContext.CompositionContext", Justification="Is accessible by the corresponding typed property.")]

namespace TomsToolbox.Wpf.Composition
{
    using System.ComponentModel;

    using TomsToolbox.Desktop;

    /// <summary>
    /// Base class for a view model with a typed composition context.
    /// </summary>
    /// <typeparam name="T">The type of the composition context</typeparam>
    public abstract class ComposablePartWithContext<T> : ObservableObject, IComposablePartWithContext
        where T : class
    {
        /// <summary>
        /// Gets or sets the composition context.
        /// </summary>
        object IComposablePartWithContext.CompositionContext
        {
            get
            {
                return CompositionContext;
            }
            set
            {
                var oldValue = CompositionContext;
                var newValue = value as T;

                if (oldValue == newValue)
                    return;

                CompositionContext = newValue;
                OnCompositionContextChanged(oldValue, newValue);
            }
        }

        /// <summary>
        /// Gets the composition context.
        /// </summary>
        public T CompositionContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the composition context has changed.
        /// The base implementations raised the <see cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnCompositionContextChanged(T oldValue, T newValue)
        {
            OnPropertyChanged(() => CompositionContext);
        }
    }
}
