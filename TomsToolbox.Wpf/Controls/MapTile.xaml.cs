namespace TomsToolbox.Wpf.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Represents on tile in the map.
    /// </summary>
    public class MapTile : Viewbox, IMapTile, ILayer
    {
        /// <summary>
        /// The size of one tile in pixels.
        /// </summary>
        public const double TileSize = 256;
        private static readonly Rect TileRect = new Rect(0, 0, TileSize, TileSize);
        private Size _subLevelTreshold = (Size)(1.5 * TileSize * new Vector(1, 1));

        [CanBeNull] private readonly IMapTile _parent;
        private readonly int _x;
        private readonly int _y;
        private readonly int _zoomLevel;

        private readonly Panel _world = new Grid { Width = TileSize, Height = TileSize };
        private readonly Panel _subLevel = new UniformGrid { Rows = 2, Columns = 2 };

        /// <summary>
        /// Initializes a new root instance of the <see cref="MapTile"/> class.
        /// </summary>
        public MapTile()
            : this(null, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTile"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private MapTile([CanBeNull] IMapTile parent, int x, int y)
        {
            _parent = parent;

            if (parent != null)
            {
                _x = 2 * parent.X + x;
                _y = 2 * parent.Y + y;
                _zoomLevel = parent.ZoomLevel + 1;
            }

            InitializeComponent();

            Loaded += (_, __) =>
            {
                var designUnitSize = this.GetDesignUnitSize();
                _subLevelTreshold = (Size)(1.5 * TileSize * (Vector)designUnitSize);
                Invalidate();
            };
        }

        /// <summary>
        /// Gets the horizontal index of this tile.
        /// </summary>
        public int X => _x;

        /// <summary>
        /// Gets the vertical index of this tile.
        /// </summary>
        public int Y => _y;

        /// <summary>
        /// Gets the zoom level of this tile.
        /// </summary>
        public int ZoomLevel => _zoomLevel;

        /// <summary>
        /// Gets the logical parent  element of this element.
        /// </summary>
        IMapTile IMapTile.Parent => _parent;

        /// <summary>
        /// Unloads this instance when the tile is no longer visible.
        /// </summary>
        public void Unload()
        {
            Image = null;

            SubTiles.ForEach(subTile => subTile?.Unload());
            // ReSharper disable once PossibleNullReferenceException
            _subLevel?.Children.Clear();
        }

        /// <summary>
        /// Gets or sets the viewport where the map will be displayed.
        /// </summary>
        [CanBeNull]
        public FrameworkElement Viewport
        {
            get => (FrameworkElement)GetValue(ViewportProperty);
            set => SetValue(ViewportProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Viewport"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(FrameworkElement), typeof(MapTile));

        /// <summary>
        /// Gets or sets the image provider that can load the images.
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
        [NotNull]
        public static readonly DependencyProperty ImageProviderProperty =
            DependencyProperty.Register("ImageProvider", typeof(IImageProvider), typeof(MapTile), new FrameworkPropertyMetadata((sender, e) => ((MapTile)sender)?.ImageProvider_Changed()));

        /// <summary>
        /// Gets or sets the image for this tile.
        /// </summary>
        [CanBeNull]
        public IImage Image
        {
            get => (IImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Image"/> dependency property
        /// </summary>
        [NotNull]
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(IImage), typeof(MapTile), new FrameworkPropertyMetadata((sender, e) => Disposable.Dispose(e.OldValue)));

        /// <summary>
        /// Invalidates the layout of this instance.
        /// </summary>
        public void Invalidate()
        {
            if (!IsLoaded || (Parent == null))
                return;

            var viewPort = Viewport;
            var world = _world;

            if ((viewPort == null) || (world == null))
                return;

            if (!IsThisTileVisible(world, viewPort, out var extent))
            {
                Unload();
                return;
            }

            var imageProvider = ImageProvider;
            if (imageProvider == null)
                return;

            var isSubLevelVisible = IsSubLevelVisible(extent, imageProvider.MaxZoom);

            if ((ZoomLevel >= imageProvider.MinZoom) && !isSubLevelVisible)
            {
                if (Image == null)
                {
                    Image = imageProvider.GetImage(this);
                    Image.Loaded += (_, __) => this.BeginInvoke(Invalidate);
                }

                // ReSharper disable once PossibleNullReferenceException
                if (!Image.IsLoaded)
                    return;

                SubTiles.ForEach(subTile => subTile?.Unload());
                // ReSharper disable once PossibleNullReferenceException
                _subLevel?.Children.Clear();
                return;
            }

            ForceSubLevel(this, _subLevel);
        }

        [NotNull, ItemNotNull]
        private IEnumerable<IMapTile> SubTiles
        {
            get
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return _subLevel?.Children.Cast<IMapTile>() ?? Enumerable.Empty<IMapTile>();
            }
        }

        private void InitializeComponent()
        {
            Stretch = Stretch.Uniform;

            var image = new Image { Stretch = Stretch.None };
            BindingOperations.SetBinding(image, System.Windows.Controls.Image.SourceProperty, new Binding(nameof(Image) + "." + nameof(Image.Source)) { IsAsync = true, Source = this });

            _world.Children.Add(image);
            _world.Children.Add(_subLevel);

            Child = _world;
        }

        private void ImageProvider_Changed()
        {
            Image = null;
            Invalidate();
        }

        private static void ForceSubLevel([NotNull] IMapTile tile, [CanBeNull] Panel subLevel)
        {
            if (subLevel == null)
                return;

            subLevel.Visibility = Visibility.Visible;

            if (subLevel.Children.Count > 0)
                return;

            for (var y = 0; y < 2; y++)
            {
                for (var x = 0; x < 2; x++)
                {
                    var mapTile = new MapTile(tile, x, y);

                    BindingOperations.SetBinding(mapTile, ViewportProperty, new Binding { Path = new PropertyPath(ViewportProperty), Source = tile });
                    BindingOperations.SetBinding(mapTile, ImageProviderProperty, new Binding { Path = new PropertyPath(ImageProviderProperty), Source = tile });

                    subLevel.Children.Add(mapTile);
                }
            }
        }

        private bool IsSubLevelVisible(Size extent, int maxZoom)
        {
            return (_zoomLevel < maxZoom) && (extent.Width > _subLevelTreshold.Width) && (extent.Height > _subLevelTreshold.Height);
        }

        private static bool IsThisTileVisible([NotNull] Visual visual, [NotNull] FrameworkElement viewPort, out Size extent)
        {

            var tileRect = TileRect.Translate(visual, viewPort);
            var viewPortRect = viewPort.GetClientRect();

            extent = tileRect.Size;

            return tileRect.IntersectsWith(viewPortRect);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "{" + X + "," + Y + "," + ZoomLevel + "}";
        }
    }
}
