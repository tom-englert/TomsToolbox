namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Threading;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Interactivity;
    using TomsToolbox.Wpf.XamlExtensions;

    /// <summary>
    /// Base class to implement visual composition behaviors.
    /// </summary>
    /// <typeparam name="T">The type the VisualCompositionBehavior can be attached to.</typeparam>
    /// <remarks>
    /// ViewModels declare themselves as candidates for visual composition by adding the <see cref="VisualCompositionExportAttribute"/>, 
    /// and the visual composition behaviors dynamically link the view models to the views with the matching region ids.
    /// </remarks>
    public abstract class VisualCompositionBehavior<T> : FrameworkElementBehavior<T>, IVisualCompositionBehavior
        where T : FrameworkElement
    {
        private readonly DispatcherThrottle _deferredUpdateThrottle;
        private INotifyChanged _exportProviderChangeTracker;
        private ExportProvider _exportProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCompositionBehavior{T}"/> class.
        /// </summary>
        protected VisualCompositionBehavior()
        {
            _deferredUpdateThrottle = new DispatcherThrottle(DispatcherPriority.Background, Update);
        }

        /// <summary>
        /// Gets or sets the id of the region. The region id is used to select candidates for composition.
        /// </summary>
        public string RegionId
        {
            get { return (string)GetValue(RegionIdProperty); }
            set { SetValue(RegionIdProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="RegionId"/> dependency property
        /// </summary>
        public static readonly DependencyProperty RegionIdProperty =
            DependencyProperty.Register("RegionId", typeof(string), typeof(VisualCompositionBehavior<T>), new FrameworkPropertyMetadata((sender, e) => ((VisualCompositionBehavior<T>)sender).RegionId_Changed()));


        /// <summary>
        /// Gets or sets the composition context.
        /// </summary>
        public object CompositionContext
        {
            get { return GetValue(CompositionContextProperty); }
            set { SetValue(CompositionContextProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CompositionContext"/> dependency property
        /// </summary>
        public static readonly DependencyProperty CompositionContextProperty =
            DependencyProperty.Register("CompositionContext", typeof(object), typeof(VisualCompositionBehavior<T>), new FrameworkPropertyMetadata(null, (sender, e) => ((VisualCompositionBehavior<T>)sender).Update()));


        /// <summary>
        /// Gets or sets the region identifier binding; 
        /// the binding will be applied to the <see cref="RegionId"/> property only after the behavior is attached to the logical tree, so you don't get misleading binding errors.
        /// </summary>
        /// <remarks>
        /// Use this property instead of setting a direct binding to the <see cref="RegionId"/> property if the direct binding will generate binding error message, e.g. in style setters.
        /// </remarks>
        public BindingBase RegionIdBinding
        {
            get { return (BindingBase)GetValue(_regionIdBindingProperty); }
            set { SetValue(_regionIdBindingProperty, value); }
        }

        /// <summary>
        /// Backing "field" for the <see cref="RegionIdBinding"/> property. 
        /// Internally it must be a <see cref="DependencyProperty"/>, else <see cref="Freezable.Clone"/> would not clone it, 
        /// but for the framework the <see cref="RegionIdBinding"/> property must look like a regular property, else it would try to apply the binding instead of simply assigning it.
        /// </summary>
        private static readonly DependencyProperty _regionIdBindingProperty =
            DependencyProperty.Register("InternalRegionIdBinding", typeof(BindingBase), typeof(VisualCompositionBehavior<T>));


        /// <summary>
        /// Gets or sets the composition context binding.
        /// the binding will be applied to the <see cref="RegionId"/> property only after the behavior is attached to the logical tree, so you don't get misleading binding errors.
        /// </summary>
        /// <remarks>
        /// Use this property instead of setting a direct binding to the <see cref="RegionId"/> property if the direct binding will generate binding error message, e.g. in style setters.
        /// </remarks>
        public BindingBase CompositionContextBinding
        {
            get { return (BindingBase)GetValue(_compositionContextBindingProperty); }
            set { SetValue(_compositionContextBindingProperty, value); }
        }

        /// <summary>
        /// Backing "field" for the <see cref="CompositionContextBinding"/> property. 
        /// Internally it must be a <see cref="DependencyProperty"/>, else <see cref="Freezable.Clone"/> would not clone it, 
        /// but for the framework the <see cref="CompositionContextBinding"/> property must look like a regular property, else it would try to apply the binding instead of simply assigning it.
        /// </summary>
        private static readonly DependencyProperty _compositionContextBindingProperty =
            DependencyProperty.Register("InternalCompositionContextBinding", typeof(BindingBase), typeof(VisualCompositionBehavior<T>));

        /// <summary>
        /// Gets or sets the export provider (IOC). The export provider must be registered with the <see cref="ExportProviderLocator"/>.
        /// </summary>
        protected ExportProvider ExportProvider
        {
            get
            {
                return InternalExportProvider ?? (InternalExportProvider = GetExportProvider());
            }
            private set
            {
                InternalExportProvider = value;
            }
        }

        private ExportProvider InternalExportProvider
        {
            get
            {
                return _exportProvider;
            }
            set
            {
                if (_exportProvider != null)
                {
                    _exportProvider.ExportsChanged -= ExportProvider_ExportsChanged;
                }

                _exportProvider = value;

                if (_exportProvider != null)
                {
                    _exportProvider.ExportsChanged += ExportProvider_ExportsChanged;
                    Update();
                }
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

            if (RegionIdBinding != null)
                BindingOperations.SetBinding(this, RegionIdProperty, RegionIdBinding);

            if (CompositionContextBinding != null)
                BindingOperations.SetBinding(this, CompositionContextProperty, CompositionContextBinding);

            var associatedObject = AssociatedObject;
            Contract.Assume(associatedObject != null);

            if (DesignerProperties.GetIsInDesignMode(associatedObject))
                return;

            _exportProviderChangeTracker = associatedObject.Track(ExportProviderLocator.ExportProviderProperty);
            _exportProviderChangeTracker.Changed += ExportProvider_Changed;

            ExportProvider = associatedObject.TryGetExportProvider();
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_exportProviderChangeTracker != null)
                _exportProviderChangeTracker.Changed -= ExportProvider_Changed;

            ExportProvider = null;
        }

        /// <summary>
        /// Gets the visual composition exports for the specified region.
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        /// <returns>The exports for the region, or <c>null</c> if the export provider is not set yet.</returns>
        protected IEnumerable<Lazy<object, IVisualCompositionMetadata>> GetExports(string regionId)
        {
            return ExportProvider?.GetExports<object, IVisualCompositionMetadata>(VisualCompositionExportAttribute.ExportContractName)
                .Where(item => item.Metadata.TargetRegions.Contains(regionId));
        }

        /// <summary>
        /// Gets the target object for the item. 
        /// If the item implements <see cref="IComposablePartFactory"/>, the element returned by the factory is returned; 
        /// otherwise the item itself is returned.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item or the factory generated item.</returns>
        protected object GetTarget(object item)
        {
            var partFactory = item as IComposablePartFactory;

            return (partFactory != null) ? partFactory.GetPart(CompositionContext) : item;
        }

        /// <summary>
        /// Called when any of the constraints have changed and the target needs to be updated.
        /// </summary>
        protected void Update()
        {
            try
            {
                OnUpdate();
            }
            catch (Exception ex)
            {
                VisualComposition.OnError(this, ex);
            }
        }

        /// <summary>
        /// Called when any of the constraints have changed and the target needs to be updated.
        /// </summary>
        /// <remarks>
        /// Derived classes override this to update the target element.
        /// </remarks>
        protected abstract void OnUpdate();

        private ExportProvider GetExportProvider()
        {
            var associatedObject = AssociatedObject;
            if (associatedObject == null)
                return null;

            var exportProvider = associatedObject.TryGetExportProvider();

            if (IsLoaded && (exportProvider == null))
            {
                VisualComposition.OnError(this, associatedObject.GetMissingExportProviderMessage());
            }

            return exportProvider;
        }

        private void RegionId_Changed()
        {
            Update();
        }

        private void ExportProvider_ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            // Defer update using a throttle:
            // - Export events may come from any thread, must dispatch to UI thread anyhow.
            // - Adding many catalogs will result in many ExportsChanged events.
            _deferredUpdateThrottle.Tick();
        }

        private void ExportProvider_Changed(object sender, EventArgs e)
        {
            ExportProvider = AssociatedObject?.TryGetExportProvider();
        }
    }
}
