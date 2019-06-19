namespace TomsToolbox.Wpf
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop;

    /// <summary>
    /// Support binding to a property of an element when the target is not a <see cref="DependencyObject"/>
    /// </summary>
    /// <typeparam name="T">The type of the variable.</typeparam>
    public class PropertyBinding<T>
    {
        [NotNull]
        private readonly BindingHelper _bindingHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="path">The path.</param>
        public PropertyBinding([CanBeNull] object source, [CanBeNull] string path)
            : this(source, BindingMode.OneWay, path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyPath">The property path.</param>
        public PropertyBinding([CanBeNull] object source, [CanBeNull] PropertyPath propertyPath)
            : this(source, BindingMode.OneWay, propertyPath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        public PropertyBinding([CanBeNull] object source, BindingMode mode, [CanBeNull] string path)
            : this(source, mode, new PropertyPath(path))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{T}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="propertyPath">The property path.</param>
        public PropertyBinding([CanBeNull] object source, BindingMode mode, [CanBeNull] PropertyPath propertyPath)
        {
            _bindingHelper = new BindingHelper(this);
            BindingOperations.SetBinding(_bindingHelper, BindingHelper.ValueProperty, new Binding { Path = propertyPath, Source = source, Mode = mode });
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [CanBeNull]
        public T Value
        {
            get => _bindingHelper.GetValue<T>(BindingHelper.ValueProperty);
            set => _bindingHelper.SetValue(BindingHelper.ValueProperty, value);
        }

        /// <summary>
        /// Occurs when the value has changed.
        /// </summary>
        public event EventHandler<PropertyBindingValueChangedEventArgs<T>> ValueChanged;

        private void Value_Changed([CanBeNull] T oldValue, [CanBeNull] T newValue)
        {
            ValueChanged?.Invoke(this, new PropertyBindingValueChangedEventArgs<T>(oldValue, newValue));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Detach()
        {
            BindingOperations.ClearBinding(_bindingHelper, BindingHelper.ValueProperty);
        }

        private class BindingHelper : DependencyObject
        {
            [NotNull]
            private readonly PropertyBinding<T> _owner;

            public BindingHelper([NotNull] PropertyBinding<T> owner)
            {
                _owner = owner;
            }

            [NotNull] public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(T), typeof(BindingHelper), new FrameworkPropertyMetadata((sender, e) => ((BindingHelper)sender)?._owner.Value_Changed((T)e.OldValue, (T)e.NewValue)));
        }
    }
}
