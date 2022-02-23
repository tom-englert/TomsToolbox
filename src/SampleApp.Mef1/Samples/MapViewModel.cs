#pragma warning disable CCRSI_CreateContractInvariantMethod // Missing Contract Invariant Method.

namespace SampleApp.Mef1.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using PropertyChanged;

    using SampleApp.Mef1.Map;

    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition.Mef;
    using TomsToolbox.Wpf.Controls;

    [VisualCompositionExport(RegionId.Main, Sequence = 1)]
    [AddINotifyPropertyChangedInterface]
    public class MapViewModel
    {
        private static readonly string _configurationFileName = Path.Combine(BaseDirectory, "Map", "MapSources.xml");

        private static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory ?? ".";

        private readonly MapSourceFile _mapSourceFile;

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

        public IList<MapSource>? MapSources => _mapSourceFile.MapSources;

        public IImageProvider? ImageProvider { get; set; }

        public Coordinates Center { get; set; } = new(52.5075419, 13.4251364);

        public Coordinates MousePosition { get; set; }

        public Poi? SelectedPoi { get; set; }

#pragma warning disable IDE0051 // Remove unused private members
        private void OnSelectedPoiChanged()
#pragma warning restore IDE0051 // Remove unused private members
        {
            if (SelectedPoi != null)
            {
                Center = SelectedPoi.Coordinates;

            }
        }

        public IList<Poi> Pois { get; } =  new ObservableCollection<Poi>
        {
            new() {Coordinates = new Coordinates(52.3747158, 4.8986142), Description = "Amsterdam"},
            new() {Coordinates = new Coordinates(52.5075419, 13.4251364), Description = "Berlin"},
            new() {Coordinates = new Coordinates(55.749792, 37.632495), Description = "Moscow"},
            new() {Coordinates = new Coordinates(40.7033127, -73.979681), Description = "New York"},
            new() {Coordinates = new Coordinates(41.9100711, 12.5359979), Description = "Rome"},
        };

        public Rect Bounds { get; set; }

        [Description(nameof(OnSelectionChanged))]
        public Rect Selection { get; set; } = Rect.Empty;

        private void OnSelectionChanged()
        {
            var value = Selection;
            if (value.IsEmpty)
                return;

            // Sample: Transform to WGS-84:
            // ReSharper disable UnusedVariable => just show how to use this..
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            var topLeft = (Coordinates)value.TopLeft;
            var bottomRight = (Coordinates)value.BottomRight;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
            // ReSharper restore UnusedVariable
        }

        public ICommand ClearSelectionCommand => new DelegateCommand(() => !Selection.IsEmpty, () => Selection = Rect.Empty);

        public ICommand MouseDoubleClickCommand => new DelegateCommand<Point>(p => Pois.Add(new Poi {Coordinates = p, Description = "New Poi"}));

        public override string ToString()
        {
            return "Map";
        }
    }
}