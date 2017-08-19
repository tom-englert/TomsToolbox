namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;

    /// <summary>
    /// A control showing maps represented in a tile system, e.g. Open Street Map, Bing or Google maps.
    /// See e.g. <see href="https://msdn.microsoft.com/en-us/library/bb259689.aspx"/> for a description how the tile system works.
    /// </summary>
    [TemplatePart(Name = TemplatePartWorld)]
    [TemplatePart(Name = TemplatePartViewport)]
    public class Map : ContentControl
    {
        /// <summary>
        /// The template part name of the world visual.
        /// </summary>
        public const string TemplatePartWorld = @"PART_World";
        /// <summary>
        /// The template part name of the viewport visual.
        /// </summary>
        public const string TemplatePartViewport = @"PART_Viewport";

        private static readonly Point LogicalCenter = new Point(0.5, 0.5);

        [NotNull]
        private readonly DispatcherThrottle _updateThrottle;

        private bool _isUpdating;

        static Map()
        {
            DefaultStyleKeyProperty?.OverrideMetadata(typeof(Map), new FrameworkPropertyMetadata(typeof(Map)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Map" /> class.
        /// </summary>
        public Map()
        {
            Focusable = false;
            ClipToBounds = true;
            _updateThrottle = new DispatcherThrottle(DispatcherPriority.Background, Update);

            Loaded += (_, __) => ViewportSize = (Vector)this.GetPhysicalPixelSize() * 512.0;
        }

        #region Dependency Properties

        /// <summary>
        /// Gets the zoom factor.
        /// </summary>
        public Vector ViewportSize
        {
            get => this.GetValue<Vector>(ViewportSizeProperty);
            private set => SetValue(ViewportSizePropertyKey, value);
        }
        [NotNull] private static readonly DependencyPropertyKey ViewportSizePropertyKey =
            DependencyProperty.RegisterReadOnly("ViewportSize", typeof(Vector), typeof(Map), new FrameworkPropertyMetadata(new Vector(512, 512)));
        /// <summary>
        /// Identifies the <see cref="ViewportSize"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty ViewportSizeProperty = ViewportSizePropertyKey.DependencyProperty;


        /// <summary>
        /// Gets or sets the image provider.
        /// </summary>
        [CanBeNull]
        public IImageProvider ImageProvider
        {
            get => (IImageProvider)GetValue(ImageProviderProperty);
            set => SetValue(ImageProviderProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="ImageProvider"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty ImageProviderProperty =
            DependencyProperty.Register("ImageProvider", typeof(IImageProvider), typeof(Map));

        /// <summary>
        /// Gets the zoom factor.
        /// </summary>
        public double ZoomFactor
        {
            get => this.GetValue<double>(ZoomFactorProperty);
            private set => SetValue(ZoomFactorPropertyKey, value);
        }
        [NotNull] private static readonly DependencyPropertyKey ZoomFactorPropertyKey =
            DependencyProperty.RegisterReadOnly("ZoomFactor", typeof(double), typeof(Map), new FrameworkPropertyMetadata(1.0, (sender, e) => ((Map)sender)?.ZoomFactor_Changed()));
        /// <summary>
        /// Identifies the <see cref="ZoomFactor"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty ZoomFactorProperty = ZoomFactorPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the zoom level. Set to integer values to get non-blurred images.
        /// </summary>
        public double ZoomLevel
        {
            get => this.GetValue<double>(ZoomLevelProperty);
            set => SetValue(ZoomLevelProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="ZoomLevel"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(Map), new FrameworkPropertyMetadata(0.0, (sender, e) => ((Map)sender)?.ZoomLevel_Changed((double)e.NewValue), (d, baseValue) => ((Map)d)?.ZoomLevel_CoerceValue(baseValue.SafeCast<double>())));

        /// <summary>
        /// Gets or sets the logical point of the map that is displayed in the center of the viewport.
        /// </summary>
        public Point Center
        {
            get => this.GetValue<Point>(CenterProperty);
            set => SetValue(CenterProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point), typeof(Map), new FrameworkPropertyMetadata(LogicalCenter, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => ((Map)sender)?.Center_Changed((Point)e.NewValue), (d, baseValue) => Center_CoerceValue(baseValue.SafeCast<Point>())));

        /// <summary>
        /// Gets the logical offset that the map image is moved relative to it's origin.
        /// </summary>
        public Vector Offset
        {
            get => this.GetValue<Vector>(OffsetProperty);
            private set => SetValue(OffsetPropertyKey, value);
        }
        [NotNull] private static readonly DependencyPropertyKey OffsetPropertyKey =
            DependencyProperty.RegisterReadOnly("Offset", typeof(Vector), typeof(Map), new FrameworkPropertyMetadata((sender, e) => ((Map)sender)?.Offset_Changed()));
        /// <summary>
        /// Identifies the <see cref="Offset"/> read only dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty OffsetProperty = OffsetPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the logical coordinates of the zooming point that will stay fixed when zooming.
        /// </summary>
        public Point ZoomingPoint
        {
            get => this.GetValue<Point>(ZoomingPointProperty);
            set => SetValue(ZoomingPointProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="ZoomingPoint"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty ZoomingPointProperty =
            DependencyProperty.Register("ZoomingPoint", typeof(Point), typeof(Map), new FrameworkPropertyMetadata(LogicalCenter, (sender, e) => ((Map)sender)?.ZoomingPoint_Changed((Point)e.OldValue, (Point)e.NewValue)));

        /// <summary>
        /// Gets the bounds of the viewport in logical coordinates.
        /// </summary>
        /// <remarks>
        /// This property only has a public setter to enable binding; changing this property wont have any effect.
        /// </remarks>
        public Rect Bounds
        {
            get => this.GetValue<Rect>(BoundsProperty);
            set => SetValue(BoundsProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Bounds"/> dependency property
        /// </summary>
        [NotNull] public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register("Bounds", typeof(Rect), typeof(Map), new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets the element representing the map layer.
        /// </summary>
        [CanBeNull]
        public FrameworkElement World
        {
            get => (FrameworkElement)GetValue(WorldProperty);
            private set => SetValue(WorldPropertyKey, value);
        }
        [NotNull] private static readonly DependencyPropertyKey WorldPropertyKey =
            DependencyProperty.RegisterReadOnly("World", typeof(FrameworkElement), typeof(Map), new FrameworkPropertyMetadata());
        /// <summary>
        /// Identifies the <see cref="World"/> dependency property.
        /// </summary>
        [NotNull] public static readonly DependencyProperty WorldProperty = WorldPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the element representing the viewport.
        /// </summary>
        [CanBeNull]
        public FrameworkElement Viewport
        {
            get => (FrameworkElement)GetValue(ViewportProperty);
            private set => SetValue(ViewportPropertyKey, value);
        }
        [NotNull] private static readonly DependencyPropertyKey ViewportPropertyKey =
            DependencyProperty.RegisterReadOnly("Viewport", typeof(FrameworkElement), typeof(Map), new FrameworkPropertyMetadata());
        /// <summary>
        /// Identifies the <see cref="Viewport"/> dependency property.
        /// </summary>
        [NotNull] public static readonly DependencyProperty ViewportProperty = ViewportPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the last mouse position when the mouse was over the map in logical coordinates.
        /// </summary>
        /// <remarks>
        /// This property only has a public setter to enable binding; changing this property wont have any effect.
        /// </remarks>
        public Point MousePosition
        {
            get => this.GetValue<Point>(MousePositionProperty);
            set => SetValue(MousePositionProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="MousePosition"/> dependency property.
        /// </summary>
        [NotNull] public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register("MousePosition", typeof(Point), typeof(Map), new FrameworkPropertyMetadata(LogicalCenter, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// Gets or sets the double click command. The command will be executed when the user double clicks on the map.
        /// The command parameter is a <see cref="Point"/> containing the logical coordinates.
        /// </summary>
        [CanBeNull]
        public ICommand MouseDoubleClickCommand
        {
            get => (ICommand)GetValue(MouseDoubleClickCommandProperty);
            set => SetValue(MouseDoubleClickCommandProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="MouseDoubleClickCommand"/> dependency property.
        /// </summary>
        [NotNull] public static readonly DependencyProperty MouseDoubleClickCommandProperty =
            DependencyProperty.Register("MouseDoubleClickCommand", typeof(ICommand), typeof(Map));

        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var template = Template;
            if (template == null)
                return;

            World = template.FindName(TemplatePartWorld, this) as FrameworkElement;
            Viewport = template.FindName(TemplatePartViewport, this) as FrameworkElement;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged" /> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _updateThrottle.Tick();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            var layer = World;
            if (layer == null)
                return;

            MousePosition = e.GetPosition(layer);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Control.MouseDoubleClick" /> routed event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            var layer = World;
            if (layer == null)
                return;

            var command = MouseDoubleClickCommand;
            if (command == null)
                return;

            var mousePosition = e.GetPosition(layer);
            command.Execute(mousePosition);
        }

        private void ZoomLevel_Changed(double newValue)
        {
            ZoomFactor = Math.Pow(2, newValue);

            _updateThrottle.Tick();
        }

        private double ZoomLevel_CoerceValue(double baseValue)
        {
            var minLevel = ImageProvider?.MinZoom ?? 0;
            var maxLevel = ImageProvider?.MaxZoom ?? 15;

            return baseValue.Clip(minLevel, maxLevel);
        }

        private void ZoomFactor_Changed()
        {
            _updateThrottle.Tick();
        }

        private void Center_Changed(Point newValue)
        {
            if (_isUpdating)
                return;

            ZoomingPoint = LogicalCenter;
            Offset = (LogicalCenter - newValue) * ZoomFactor;

            _updateThrottle.Tick();
        }

        private static Point Center_CoerceValue(Point baseValue)
        {
            return new Point(baseValue.X.Clip(0, 1), baseValue.Y.Clip(0, 1));
        }

        private void Offset_Changed()
        {
            _updateThrottle.Tick();
        }

        private void ZoomingPoint_Changed(Point oldValue, Point newValue)
        {
            Offset += (oldValue - newValue) * (1 - ZoomFactor);

            _updateThrottle.Tick();
        }

        private void Update()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            var layer = World;
            if (layer != null)
            {
                _isUpdating = true;
                Bounds = this.GetClientRect(layer);
                Center = Bounds.GetCenter();
                _isUpdating = false;
            }

            foreach (var item in this.VisualDescendants().OfType<ILayer>())
            {
                // ReSharper disable once PossibleNullReferenceException
                item.Invalidate();
            }
        }
    }
}
