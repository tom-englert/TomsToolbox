namespace SampleApp.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using SampleApp.Map;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;
    using TomsToolbox.Wpf.Controls;

    [VisualCompositionExport(RegionId.Main, Sequence = 1)]
    public class MapViewModel : ObservableObject
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        [NotNull] private static readonly string _configurationFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Map", "MapSources.xml");

        [NotNull]
        private readonly MapSourceFile _mapSourceFile;

        private readonly ObservableCollection<Poi> _pois = new ObservableCollection<Poi>
        {
            new Poi { Coordinates = new Coordinates(52.3747158, 4.8986142), Description = "Amsterdam" },
            new Poi { Coordinates = new Coordinates(52.5075419, 13.4251364), Description = "Berlin" },
            new Poi { Coordinates = new Coordinates(55.749792, 37.632495), Description = "Moscow" },
            new Poi { Coordinates = new Coordinates(40.7033127, -73.979681), Description = "New York" },
            new Poi { Coordinates = new Coordinates(41.9100711, 12.5359979), Description = "Rome" },
        };

        private IImageProvider _imageProvider;
        private Coordinates _center = new Coordinates(52.5075419, 13.4251364);
        private Poi _selectedPoi;
        private Coordinates _mousePosition;
        private Rect _selection = Rect.Empty;
        private Rect _bounds;

        public MapViewModel()
        {
            try
            {
                _mapSourceFile = MapSourceFile.Load(_configurationFileName);
                ImageProvider = _mapSourceFile.MapSources?.FirstOrDefault();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        [CanBeNull]
        public IList<MapSource> MapSources => _mapSourceFile.MapSources;

        [CanBeNull]
        public IImageProvider ImageProvider
        {
            get
            {
                return _imageProvider;
            }
            set
            {
                SetProperty(ref _imageProvider, value, () => ImageProvider);
            }
        }

        public Coordinates Center
        {
            get
            {
                return _center;
            }
            set
            {
                SetProperty(ref _center, value, () => Center);
            }
        }

        public Coordinates MousePosition
        {
            get
            {
                return _mousePosition;
            }
            set
            {
                SetProperty(ref _mousePosition, value, () => MousePosition);
            }
        }

        [CanBeNull]
        public Poi SelectedPoi
        {
            get
            {
                return _selectedPoi;
            }
            set
            {
                if (SetProperty(ref _selectedPoi, value, () => SelectedPoi) && value != null)
                {
                    Center = value.Coordinates;
                }
            }
        }

        [CanBeNull]
        public IList<Poi> Pois => _pois;

        public Rect Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                SetProperty(ref _bounds, value,() => Bounds);
            }
        }

        public Rect Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (SetProperty(ref _selection, value, () => Selection))
                {
                    if (!value.IsEmpty)
                    {
                        // Sample: Transform to WGS-84:
                        // ReSharper disable UnusedVariable => just show how to use this..
                        var topLeft = (Coordinates)value.TopLeft;
                        var bottomRight = (Coordinates)value.BottomRight;
                        // ReSharper restore UnusedVariable
                    }
                }
            }
        }

        [NotNull]
        public ICommand ClearSelectionCommand
        {
            get
            {
                return new DelegateCommand(() => !Selection.IsEmpty, () => Selection = Rect.Empty);
            }
        }

        [NotNull]
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                return new DelegateCommand<Point>(p => _pois.Add(new Poi { Coordinates = p, Description = "New Poi" }));
            }
        }

        public override string ToString()
        {
            return "Map";
        }
    }
}
