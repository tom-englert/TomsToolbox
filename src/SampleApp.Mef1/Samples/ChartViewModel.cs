namespace SampleApp.Mef1.Samples
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    using PropertyChanged;

    using SampleApp.Mef1;

    using TomsToolbox.Wpf.Composition.Mef;

    [Export]
    [VisualCompositionExport(RegionId.Main, Sequence = 1.5)]
    [AddINotifyPropertyChangedInterface]
    internal class ChartViewModel
    {
        public override string ToString()
        {
            return "Chart";
        }

        public ICollection<DataPoint> Points => Array.Empty<Point>()
            .Concat(GetSinusPoints(0.0, 0.5))
            .Concat(GetDistinctPoints())
            .Select(item => new DataPoint(item, Brushes.Orange) { Label = item.X.ToString("F2", CultureInfo.CurrentCulture) + ":" + item.Y.ToString("F2", CultureInfo.CurrentCulture) })
            .ToArray();

        public ICollection<DataLine> Lines => GetLines().ToList();

        private IEnumerable<DataLine> GetLines()
        {
            yield return new DataLine(GetSinusPoints(1.0).ToList(), Brushes.Blue);
            yield return new DataLine(new[] { new Point(0, 1), new Point(10, 1) }, Brushes.Orange);
            yield return new DataLine(new[] { new Point(0, -1), new Point(10, -1) }, Brushes.Orange);
            yield return new DataLine(new[] { new Point(5, 0), new Point(5, 1) }, Brushes.Orange);
        }

        private IEnumerable<Point> GetSinusPoints(double phase, double step = 0.1)
        {
            for (var x = 0.0; x < 3 * Math.PI; x += step)
            {
                yield return new Point(x, Math.Sin(x + phase));
            }
        }

        private IEnumerable<Point> GetDistinctPoints()
        {
            yield return new Point(1, 2);
            yield return new Point(-.5, -.1);
            yield return new Point(1, .5);
        }
    }
}
