namespace SampleApp.Mef1.Samples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    using TomsToolbox.Essentials;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Controls;
    using TomsToolbox.Wpf.XamlExtensions;

    public class DataLine
    {
        public DataLine(ICollection<Point> points, Brush color)
        {
            Points = points;
            Color = color;
        }

        public ICollection<Point> Points { get; }

        public Brush Color { get; }
    }

    public class DataPoint
    {
        public DataPoint(Point position, Brush color)
        {
            Position = position;
            Color = color;
        }

        public Point Position { get; }

        public Brush Color { get; }

        public string? Label { get; set; }
    }

    /// <summary>
    /// Interaction logic for ChartView.xaml
    /// </summary>
    public partial class Chart
    {
        private const FrameworkPropertyMetadataOptions MetadataOptions = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender;

        public Chart()
        {
            InitializeComponent();
        }

        public ICollection<DataLine>? Lines
        {
            get => (ICollection<DataLine>)GetValue(LinesProperty);
            set => SetValue(LinesProperty, value);
        }
        public static readonly DependencyProperty LinesProperty = DependencyProperty.Register(
            "Lines", typeof(ICollection<DataLine>), typeof(Chart),
            new FrameworkPropertyMetadata(default(ICollection<DataLine>), MetadataOptions,
                (sender, e) => ((Chart)sender).Lines_Changed()));

        public ICollection<DataPoint>? Points
        {
            get => (ICollection<DataPoint>)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points", typeof(ICollection<DataPoint>), typeof(Chart),
            new FrameworkPropertyMetadata(default(ICollection<DataPoint>), MetadataOptions,
                (sender, e) => ((Chart)sender).Points_Changed()));

        public Point Origin
        {
            get => (Point)GetValue(OriginProperty);
            set => SetValue(OriginProperty, value);
        }
        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register(
            "Origin", typeof(Point), typeof(Chart),
            new FrameworkPropertyMetadata(default(Point), MetadataOptions,
                (sender, e) => ((Chart)sender).Origin_Changed()));

        public Rect BoundingRect
        {
            get => (Rect)GetValue(BoundingRectProperty);
            private set => SetValue(BoundingRectPropertyKey, value);
        }
        private static readonly DependencyPropertyKey BoundingRectPropertyKey = DependencyProperty.RegisterReadOnly(
            "BoundingRect", typeof(Rect), typeof(Chart),
            new FrameworkPropertyMetadata(default(Rect), MetadataOptions));
        public static readonly DependencyProperty BoundingRectProperty = BoundingRectPropertyKey.DependencyProperty;

        public Rect DataBounds
        {
            get => (Rect)GetValue(DataBoundsProperty);
            private set => SetValue(DataBoundsPropertyKey, value);
        }
        private static readonly DependencyPropertyKey DataBoundsPropertyKey = DependencyProperty.RegisterReadOnly(
            "DataBounds", typeof(Rect), typeof(Chart),
            new FrameworkPropertyMetadata(default(Rect), MetadataOptions));
        public static readonly DependencyProperty DataBoundsProperty = DataBoundsPropertyKey.DependencyProperty;

        public Size Q1
        {
            get => (Size)GetValue(Q1Property);
            private set => SetValue(Q0PropertyKey, value);
        }
        private static readonly DependencyPropertyKey Q0PropertyKey = DependencyProperty.RegisterReadOnly(
            "Q1", typeof(Size), typeof(Chart),
            new FrameworkPropertyMetadata(default(Size), MetadataOptions));
        public static readonly DependencyProperty Q1Property = Q0PropertyKey.DependencyProperty;

        public Size Q3
        {
            get => (Size)GetValue(Q3Property);
            private set => SetValue(Q2PropertyKey, value);
        }
        private static readonly DependencyPropertyKey Q2PropertyKey = DependencyProperty.RegisterReadOnly(
            "Q3", typeof(Size), typeof(Chart),
            new FrameworkPropertyMetadata(default(Size), MetadataOptions));
        public static readonly DependencyProperty Q3Property = Q2PropertyKey.DependencyProperty;

        private void Lines_Changed()
        {
            CalculateBounds();
        }

        private void Points_Changed()
        {
            CalculateBounds();
        }

        private void Origin_Changed()
        {
            CalculateBounds();
        }

        private void CalculateBounds()
        {
            var dataLines = Lines ?? Array.Empty<DataLine>();
            var dataPoints = Points ?? Array.Empty<DataPoint>();

            var dataBounds = dataLines.Aggregate(Enumerable.Empty<Point>(), (points, items) => points.Concat(items.Points))
                .Concat(dataPoints.Select(item => item.Position))
                .Concat(new[] { Origin })
                .ToList()
                .GetBoundingRect();

            DataBounds = dataBounds;

            var boundingRect = new[] { Origin, dataBounds.TopLeft, dataBounds.BottomRight }
               .GetBoundingRect();

            BoundingRect = new Rect(
                Math.Floor(boundingRect.Left),
                Math.Floor(boundingRect.Top),
                Math.Ceiling(boundingRect.Width),
                Math.Ceiling(boundingRect.Height));

            Q1 = new Size(
                Math.Max(0, BoundingRect.Right - Origin.X),
                Math.Max(0, BoundingRect.Bottom - Origin.Y));

            Q3 = new Size(
                - Math.Min(0, BoundingRect.Left - Origin.X),
                - Math.Min(0, BoundingRect.Top - Origin.Y));

            this.VisualDescendants().OfType<ViewportCanvas>().ForEach(item => item.InvalidateMeasure());
        }

        private void Line_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement { DataContext: DataLine line })
                return;

            MessageBox.Show($"Clicked on a {line.Color} line.");
        }
    }
}
