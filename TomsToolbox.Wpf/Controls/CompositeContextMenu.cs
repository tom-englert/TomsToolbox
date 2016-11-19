namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// A markup extension to create a <see cref="CompositeContextMenu"/> in XAML.
    /// </summary>
    [MarkupExtensionReturnType(typeof(ContextMenu))]
    public class CompositeContextMenuExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContextMenuExtension"/> class.
        /// </summary>
        public CompositeContextMenuExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContextMenuExtension"/> class.
        /// </summary>
        /// <param name="regionId">The region identifier.</param>
        public CompositeContextMenuExtension(string regionId)
        {
            RegionId = regionId;
        }

        /// <summary>
        /// Gets or sets the region identifier.
        /// </summary>
        public string RegionId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a binding to seed the <see cref="IVisualCompositionBehavior.CompositionContext"/>.
        /// </summary>
        public Binding CompositionContextBinding
        {
            get;
            set;
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        [NotNull]
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var compositeContextMenu = new CompositeContextMenu()
            {
                RegionId = RegionId
            };

            if (CompositionContextBinding != null)
            {
                BindingOperations.SetBinding(compositeContextMenu, CompositeContextMenu.CompositionContextProperty, CompositionContextBinding);
            }

            return compositeContextMenu;
        }
    }

    /// <summary>
    /// A context menu that uses the composition framework to build it's content 
    /// dynamically by collecting all exported <see cref="CommandSourceFactory"/> objects 
    /// with the matching region.
    /// </summary>
    public class CompositeContextMenu : ContextMenu
    {
        static CompositeContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompositeContextMenu), new FrameworkPropertyMetadata(typeof(CompositeContextMenu)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContextMenu" /> class.
        /// </summary>
        public CompositeContextMenu()
        {
            var compositionBehavior = new ItemsControlCompositionBehavior();
            BindingOperations.SetBinding(compositionBehavior, ItemsControlCompositionBehavior.RegionIdProperty, new Binding() { Source = this, Path = new PropertyPath(RegionIdProperty) });

            var behaviors = Interaction.GetBehaviors(this);
            Contract.Assume(behaviors != null);
            behaviors.Add(compositionBehavior);
        }

        /// <summary>
        /// Gets or sets the region identifier for which to collect all exported <see cref="CommandSourceFactory"/> objects.
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
            DependencyProperty.Register("RegionId", typeof(string), typeof(CompositeContextMenu));


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
            DependencyProperty.Register("CompositionContext", typeof (object), typeof (CompositeContextMenu));
    }
}
