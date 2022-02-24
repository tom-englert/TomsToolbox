namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf.Interactivity;

    /// <summary>
    /// A behavior to set a dependency property of the associated object to a value retrieved from the DI container. The default target property is the <see cref="FrameworkElement.DataContextProperty"/>.
    /// </summary>
    /// <seealso cref="FrameworkElementBehavior{T}" />
    public class ImportBehavior : FrameworkElementBehavior<FrameworkElement>
    {
        private INotifyChanged? _tracker;
        private Type? _memberType;
        private string? _contractName;
        private DependencyProperty? _targetProperty = FrameworkElement.DataContextProperty;

        /// <summary>
        /// Gets or sets the exported type of the object to provide.
        /// </summary>
        public Type? MemberType
        {
            get => _memberType;
            set
            {
                _memberType = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the optional contract name of the exported object.
        /// </summary>
        public string? ContractName
        {
            get => _contractName;
            set
            {
                _contractName = value;
                Update();
            }
        }

        /// <summary>
        /// Gets or sets the target property to set. The default is <see cref="FrameworkElement.DataContextProperty"/>.
        /// </summary>
        public DependencyProperty? TargetProperty
        {
            get => _targetProperty;
            set
            {
                _targetProperty = value;
                Update();
            }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            if ((AssociatedObject == null) || DesignerProperties.GetIsInDesignMode(AssociatedObject))
                return;

            _tracker = AssociatedObject.Track(ExportProviderLocator.ExportProviderProperty);

            Update();
        }

        /// <inheritdoc />
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            if (_tracker != null)
            {
                _tracker.Changed -= ExportProvider_Changed;
                _tracker.Changed += ExportProvider_Changed;
            }
        }

        /// <inheritdoc />
        protected override void OnAssociatedObjectUnloaded()
        {
            base.OnAssociatedObjectUnloaded();

            if (_tracker != null)
            {
                _tracker.Changed -= ExportProvider_Changed;
            }
        }

        private void ExportProvider_Changed(object? sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            var memberType = MemberType;
            var dependencyProperty = TargetProperty;

            if ((memberType == null) || (dependencyProperty == null))
                return;

            var frameworkElement = AssociatedObject;
            var exportProvider = frameworkElement?.TryGetExportProvider();

            if (exportProvider == null)
                return;

            var value = exportProvider
                .GetExportedValues(memberType, ContractName)
                .FirstOrDefault();

            frameworkElement!.SetValue(dependencyProperty, value);
        }
    }
}
