namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xaml;

    using JetBrains.Annotations;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;
    using TomsToolbox.Desktop.Composition;

    /// <summary>
    /// The XAML equivalent of the <see cref="T:System.ComponentModel.Composition.ImportAttribute" />. Use like the <see cref="T:System.Windows.Markup.StaticExtension" />;
    /// uses the MEF <see cref="T:System.ComponentModel.Composition.Hosting.ExportProvider" /> to create the object.
    /// </summary>
    /// <seealso cref="System.Windows.Markup.MarkupExtension" />
    /// <inheritdoc />
    [MarkupExtensionReturnType(typeof(object))]
    public class ImportExtension : MarkupExtension
    {
        [NotNull, ItemNotNull]
        private readonly List<Setter> _setters = new List<Setter>();

        [CanBeNull]
        private object _targetObject;
        [CanBeNull]
        private object _targetProperty;
        [CanBeNull]
        private ExportProvider _exportProvider;
        [CanBeNull]
        private FrameworkElement _rootObject;
        [CanBeNull]
        private INotifyChanged _tracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Wpf.Composition.ImportExtension" /> class.
        /// </summary>
        /// <param name="memberType">Type of the member to provide.</param>
        public ImportExtension([NotNull] Type memberType)
        {
            Contract.Requires(memberType != null);
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
        public string ContractName
        {
            get;
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether recomposition is enabled when the container changes, just like <see cref="ImportAttribute.AllowRecomposition"/>.
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
                Contract.Ensures(Contract.Result<ICollection<Setter>>() != null);
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
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            Contract.Assume(serviceProvider != null);

            var rootObjectProvider = serviceProvider.GetService<IRootObjectProvider>();
            if (rootObjectProvider == null)
                return null;

            _rootObject = rootObjectProvider.RootObject as FrameworkElement;
            if (_rootObject == null)
                return null;

            if (DesignerProperties.GetIsInDesignMode(_rootObject))
                return null;

            if (AllowRecomposition)
            {
                var provideValueTarget = serviceProvider.GetService<IProvideValueTarget>();
                if (provideValueTarget != null)
                {
                    _targetObject = provideValueTarget.TargetObject;
                    _targetProperty = provideValueTarget.TargetProperty;
                }

                _tracker = _rootObject.Track(ExportProviderLocator.ExportProviderProperty);

                _rootObject.Loaded += RootObject_Loaded;
                _rootObject.Unloaded += RootObject_Unloaded;
            }
            else
            {
                _exportProvider = _rootObject.GetExportProvider();
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

            SetExportProvider(_rootObject?.TryGetExportProvider());
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
        private object Value
        {
            get
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                var value = _exportProvider?.GetExports(MemberType, null, ContractName)
                    .Select(item => item?.Value)
                    .FirstOrDefault();

                if (value == null)
                    return null;

                if (!(value is DependencyObject target))
                    return value;

                foreach (var setter in Setters)
                {
                    var binding = setter.Value as BindingBase;
                    var dependencyProperty = setter.Property;
                    if (dependencyProperty == null)
                        continue;

                    if (binding != null)
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

        private void ExportProvider_Changed(object sender, EventArgs e)
        {
            SetExportProvider(_rootObject?.TryGetExportProvider());
            UpdateTarget();
        }

        private void SetExportProvider(ExportProvider exportProvider)
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

        private void ExportProvider_ExportsChanged([CanBeNull] object sender, [CanBeNull] ExportsChangeEventArgs e)
        {
            UpdateTarget();
        }

        [ContractInvariantMethod, UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(MemberType != null);
            Contract.Invariant(_setters != null);
        }
    }
}