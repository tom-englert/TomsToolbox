namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Threading;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Interactivity;

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
            DependencyProperty.Register("InternalCompositionContextBinding", typeof (BindingBase), typeof (VisualCompositionBehavior<T>));

        /// <summary>
        /// Gets or sets the export provider (IOC). The export provider must be registered with the <see cref="ExportProviderLocator"/>.
        /// </summary>
        protected ExportProvider ExportProvider
        {
            get
            {
                Contract.Ensures(Contract.Result<ExportProvider>() != null);

                var owner = AssociatedObject;
                if (owner == null)
                    throw new InvalidOperationException("Can't get export provider before behavior is attached.");

                return owner.GetExportProvider();
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

            ExportProvider.ExportsChanged += ExportProvider_ExportsChanged;

            if (RegionIdBinding != null)
                BindingOperations.SetBinding(this, RegionIdProperty, RegionIdBinding);

            if (CompositionContextBinding != null)
                BindingOperations.SetBinding(this, CompositionContextProperty, CompositionContextBinding);

            Update();
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

            ExportProvider.ExportsChanged -= ExportProvider_ExportsChanged;
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

        /// <summary>
        /// Gets the target object for the item. 
        /// If the item implements <see cref="IComposablePartFactory"/>, the element returned by the factory is returned; 
        /// otherwise the item itself is returned.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The item or the factory generated item.</returns>
        protected object GetTarget(IComposablePart item)
        {
            var partFactory = item as IComposablePartFactory;

            return (partFactory != null) ? partFactory.GetPart(CompositionContext) : item;
        }

        /// <summary>
        /// Called when any of the constraints have changed and the target needs to be updated.
        /// </summary>
        /// <remarks>
        /// Derived classes override this to update the target element.
        /// </remarks>
        protected abstract void Update();
    }
}
