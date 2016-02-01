namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xaml;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;
    using TomsToolbox.Desktop.Composition;

    /// <summary>
    /// The XAML equivalent of the <see cref="ImportAttribute"/>. Use like the <see cref="System.Windows.Markup.StaticExtension"/>;
    /// uses the MEF <see cref="ExportProvider"/> to create the object.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public class ImportExtension : MarkupExtension
    {
        private readonly List<Setter> _setters = new List<Setter>();
        private object _targetObject;
        private object _targetProperty;
        private ExportProvider _exportProvider;
        private DependencyObject _rootObject;
        private INotifyChanged _tracker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExtension" /> class.
        /// </summary>
        /// <param name="memberType">Type of the member to provide.</param>
        public ImportExtension(Type memberType)
        {
            Contract.Requires(memberType != null);
            MemberType = memberType;
        }

        /// <summary>
        /// Gets or sets the exported type of the object to provide.
        /// </summary>
        public Type MemberType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the optional contract name of the exported object.
        /// </summary>
        public string ContractName
        {
            get;
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
            set;
        }

        /// <summary>
        /// Gets a list of setters that allow initializing dependency properties of the composed object.
        /// </summary>
        public ICollection<Setter> Setters
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<Setter>>() != null);
                return _setters;
            }
        }

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

            _rootObject = rootObjectProvider.RootObject as DependencyObject;
            if (_rootObject == null)
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
                _tracker.Changed += (_, __) => ExportProvider_Changed();

                RegisterForChangeEvents();
            }
            else
            {
                _exportProvider = _rootObject.GetExportProvider();
            }

            return Value;
        }

        private object Value
        {
            get
            {
                if (_exportProvider == null) 
                    return null;
                
                var value = _exportProvider.GetExports(MemberType, null, ContractName).Select(item => item.Value).FirstOrDefault();
                if (value == null)
                    return null;

                var target = value as DependencyObject;
                if (target == null)
                    return value;

                foreach (var setter in Setters)
                {
                    if (setter == null)
                        continue;

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

            var frameworkProperty = _targetProperty as DependencyProperty;

            if (frameworkProperty != null)
            {
                var frameworkTarget = _targetObject as DependencyObject;
                if (frameworkTarget != null)
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

        private void ExportProvider_Changed()
        {
            RegisterForChangeEvents();
            UpdateTarget();
        }

        private void RegisterForChangeEvents()
        {
            if (_exportProvider != null)
            {
                _exportProvider.ExportsChanged -= ExportProvider_ExportsChanged;
            }

            if (_rootObject == null)
            {
                _exportProvider = null;
                return;
            }

            _exportProvider = _rootObject.TryGetExportProvider();

            if (_exportProvider != null)
            {
                _exportProvider.ExportsChanged += ExportProvider_ExportsChanged;
            }
        }

        private void ExportProvider_ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            UpdateTarget();
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(MemberType != null);
        }
    }
}