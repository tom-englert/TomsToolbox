namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xaml;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;
    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf.Composition.XamlExtensions;

    /// <summary>
    /// The XAML equivalent of the <see cref="T:System.ComponentModel.Composition.ImportAttribute" />. Use like the <see cref="T:System.Windows.Markup.StaticExtension" />;
    /// uses the MEF <see cref="T:System.ComponentModel.Composition.Hosting.IExportProvider" /> to create the object.
    /// </summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    /// <inheritdoc />
    [MarkupExtensionReturnType(typeof(object))]
    public class ImportExtension : MarkupExtension
    {
        [NotNull, ItemNotNull]
        private readonly List<Setter> _setters = new List<Setter>();

        [CanBeNull]
        private object? _targetObject;
        [CanBeNull]
        private object? _targetProperty;
        [CanBeNull]
        private IExportProvider? _exportProvider;
        [CanBeNull]
        private INotifyChanged? _tracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Composition.ImportExtension" /> class.
        /// </summary>
        /// <param name="memberType">Type of the member to provide.</param>
        public ImportExtension([NotNull] Type memberType)
        {
            MemberType = memberType;
        }

        /// <summary>
        /// Gets or sets the exported type of the object to provide.
        /// </summary>
        [NotNull]
        public Type MemberType
        {
            get;
            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
            set;
        }

        /// <summary>
        /// Gets or sets the optional contract name of the exported object.
        /// </summary>
        [CanBeNull]
        public string? ContractName
        {
            get;
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether recomposition is enabled when the container changes, just like System.ComponentModel.Composition.ImportAttribute.AllowRecomposition
        /// </summary>
        /// <value>
        ///   <c>true</c> if recomposition is enabled; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool AllowRecomposition
        {
            get;
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            set;
        }

        /// <summary>
        /// Gets a list of setters that allow initializing dependency properties of the composed object.
        /// </summary>
        [NotNull, ItemNotNull]
        public ICollection<Setter> Setters
        {
            get
            {
                return _setters;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        [CanBeNull]
        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            var rootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            if (rootObjectProvider == null)
            {
                VisualComposition.OnError(this, $"Import: Service {nameof(IRootObjectProvider)} unavailable.");
                return null;
            }

            var rootObject = rootObjectProvider.RootObject as FrameworkElement ?? Application.Current.MainWindow;
            if (rootObject == null)
                return null;

            if (DesignerProperties.GetIsInDesignMode(rootObject))
                return null;

            if (AllowRecomposition)
            {
                var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                if (provideValueTarget != null)
                {
                    _targetObject = provideValueTarget.TargetObject;
                    _targetProperty = provideValueTarget.TargetProperty;
                }

                _tracker = rootObject.Track(ExportProviderLocator.ExportProviderProperty);

                rootObject.Loaded += RootObject_Loaded;
                rootObject.Unloaded += RootObject_Unloaded;
            }
            else
            {
                _exportProvider = rootObject.GetExportProvider();
            }

            return Value;
        }

        private void RootObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (_tracker != null)
            {
                _tracker.Changed -= ExportProvider_Changed;
                _tracker.Changed += ExportProvider_Changed;
            }

            SetExportProvider((sender as FrameworkElement)?.TryGetExportProvider());
        }

        private void RootObject_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_tracker != null)
            {
                _tracker.Changed -= ExportProvider_Changed;
            }

            SetExportProvider(null);
        }

        [CanBeNull]
        private object? Value
        {
            get
            {
                var value = _exportProvider?.GetExports(MemberType, ContractName)
                    .Select(item => item.Value)
                    .FirstOrDefault();

                if (value == null)
                    return null;

                if (!(value is DependencyObject target))
                    return value;

                foreach (var setter in Setters)
                {
                    var dependencyProperty = setter.Property;
                    if (dependencyProperty == null)
                        continue;

                    if (setter.Value is BindingBase binding)
                    {
                        BindingOperations.SetBinding(target, dependencyProperty, binding);
                    }
                    else
                    {
                        target.SetValue(dependencyProperty, setter.Value);
                    }
                }

                return value;
            }
        }

        private void UpdateTarget()
        {
            if (_targetObject == null)
                return;

            if (_targetProperty is DependencyProperty frameworkProperty)
            {
                if (_targetObject is DependencyObject frameworkTarget)
                {
                    frameworkTarget.SetValue(frameworkProperty, Value);
                    return;
                }
            }

            var propertyInfo = _targetProperty as PropertyInfo;
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(_targetObject, Value, null);
            }
        }

        private void ExportProvider_Changed(object? sender, EventArgs e)
        {
            SetExportProvider((sender as FrameworkElement)?.TryGetExportProvider());
            UpdateTarget();
        }

        private void SetExportProvider([CanBeNull] IExportProvider? exportProvider)
        {
            if (_exportProvider != null)
            {
                _exportProvider.ExportsChanged -= ExportProvider_ExportsChanged;
            }

            _exportProvider = exportProvider;

            if (_exportProvider != null)
            {
                _exportProvider.ExportsChanged += ExportProvider_ExportsChanged;
            }
        }

        private void ExportProvider_ExportsChanged([CanBeNull] object? sender, [CanBeNull] EventArgs? e)
        {
            UpdateTarget();
        }
    }
}