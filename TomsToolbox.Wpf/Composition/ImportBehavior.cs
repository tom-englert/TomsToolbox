using TomsToolbox.Wpf.Interactivity;

namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;

    /// <summary>
    /// A behavior to set a dependency property of the associated object to a value retrieved from the IOC. The default target property is the <see cref="FrameworkElement.DataContextProperty"/>.
    /// </summary>
    /// <seealso cref="FrameworkElementBehavior{FrameworkElement}" />
    public class ImportBehavior : FrameworkElementBehavior<FrameworkElement>
    {
        [CanBeNull]
        private INotifyChanged _tracker;
        [CanBeNull]
        private Type _memberType;
        [CanBeNull]
        private string _contractName;
        [CanBeNull]
        private DependencyProperty _targetProperty = FrameworkElement.DataContextProperty;

        /// <summary>
        /// Gets or sets the exported type of the object to provide.
        /// </summary>
        [CanBeNull]
        public Type MemberType
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
        [CanBeNull]
        public string ContractName
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
        [CanBeNull]
        public DependencyProperty TargetProperty
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

        private void ExportProvider_Changed([NotNull] object sender, [NotNull] EventArgs e)
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

            // ReSharper disable once AssignNullToNotNullAttribute
            var value = exportProvider.GetExports(memberType, null, ContractName)
                .Select(item => item?.Value)
                .FirstOrDefault();

            frameworkElement.SetValue(dependencyProperty, value);
        }
    }
}
