namespace SampleApp.Map
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml.Serialization;

    using JetBrains.Annotations;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Controls;

    [Serializable]
    [XmlRoot("root")]
    public class MapSourceFile
    {
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(MapSourceFile));

        [XmlArray("MapSourceList")]
        public MapSource[] MapSources
        {
            get;
            set;
        }

        public static MapSourceFile Load([NotNull] string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return (MapSourceFile)_serializer.Deserialize(stream);
            }
        }

        public void Save(string fileName)
        {
            using (var stream = File.OpenWrite(fileName))
            {
                _serializer.Serialize(stream, this);
            }
        }
    }

    [Serializable]
    public class MapSource : IImageProvider
    {
        private int _tileUrlIndex;

        [NonSerialized]
        private readonly AutoWeakIndexer<IMapTile, Image> _imageCache;

        public MapSource()
        {
            _imageCache = new AutoWeakIndexer<IMapTile, Image>(tile => new Image(this, tile), new DelegateEqualityComparer<IMapTile>(TileEquals, GetTileHashCode));
        }

        [XmlAttribute("ID")]
        public string Id
        {
            get;
            set;
        }

        [XmlElement]
        public string Copyright
        {
            get;
            set;
        }

        [XmlElement]
        public int MinZoom
        {
            get;
            set;
        }

        [XmlElement]
        public int MaxZoom
        {
            get;
            set;
        }

        [XmlElement]
        public string[] TileUrl
        {
            get;
            set;
        }

        public IImage GetImage(IMapTile tile)
        {
            return _imageCache[tile];
        }

        private Uri GetImageUri(IMapTile tile)
        {
            var pattern = TileUrl[_tileUrlIndex++ % TileUrl.Length];

            var str = pattern
                .Replace("%x", tile.X.ToString(CultureInfo.InvariantCulture))
                .Replace("%y", tile.Y.ToString(CultureInfo.InvariantCulture))
                .Replace("%z", tile.ZoomLevel.ToString(CultureInfo.InvariantCulture));

            return new Uri(str);
        }

        private static int GetTileHashCode(IMapTile arg)
        {
            return arg.X + 0x1000 * arg.Y + 0x1000000 * arg.ZoomLevel;
        }

        private static bool TileEquals(IMapTile left, IMapTile right)
        {
            return (left.X == right.X) && (left.Y == right.Y) && (left.ZoomLevel == right.ZoomLevel);
        }

        sealed class Image : IImage
        {
            private readonly object _sync = new object();
            private readonly MapSource _owner;
            private readonly IMapTile _mapTile;
            private BitmapImage _source;

            public Image(MapSource owner, IMapTile mapTile)
            {
                _owner = owner;
                _mapTile = mapTile;
            }

            public ImageSource Source
            {
                get
                {
                    return _source ?? DownloadBitmap();
                }
            }

            public bool IsLoaded
            {
                get
                {
                    return _source != null;
                }
            }

            public event EventHandler Loaded;

            private void OnLoaded()
            {
                var handler = Loaded;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }

            private BitmapImage DownloadBitmap()
            {
                try
                {
                    lock (_sync)
                    {
                        if (_source != null)
                            return _source;

                        var uri = _owner.GetImageUri(_mapTile);

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = WebHelper.Download(uri);
                        bitmap.EndInit();
                        bitmap.Freeze();

                        _source = bitmap;
                    }

                    OnLoaded();

                    return _source;
                }
                catch (WebException)
                {
                    return null;
                }
            }
        }
    }
}
