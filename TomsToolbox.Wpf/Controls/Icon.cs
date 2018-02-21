namespace TomsToolbox.Wpf.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;

    using JetBrains.Annotations;

    /// <summary>
    /// An image control that accepts a list of image sources and displays the image that best fits the size of the control.
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Control" />
    [TemplatePart(Name = ImagePartName, Type = typeof(Image))]
    [ContentProperty(nameof(Icon.Sources))]
    public class Icon : Control
    {
        private const string ImagePartName = "PART_Image";
        private Image _image;

        static Icon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Icon), new FrameworkPropertyMetadata(typeof(Icon)));
        }

        /// <summary>
        /// Gets or sets the optional viewport where the image will be displayed. If the viewport property is not set, the window is used.
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
        [NotNull] public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(FrameworkElement), typeof(Icon), new FrameworkPropertyMetadata((sender, e) => ((Icon)sender).Update()));

        /// <summary>
        /// Gets or sets the image sources that are candidates for the target image.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Required, is used via XAML")]
        [CanBeNull]
        public IList<ImageSource> Sources
        {
            get => (IList<ImageSource>)GetValue(SourcesProperty);
            set => SetValue(SourcesProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="Sources"/> dependency property
        /// </summary>
        [NotNull] 
        public static readonly DependencyProperty SourcesProperty = 
            DependencyProperty.Register("Sources", typeof(IList<ImageSource>), typeof(Icon), new FrameworkPropertyMetadata((sender, e) => ((Icon)sender).Update()));

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = Template?.FindName(ImagePartName, this) as Image;
        }

        /// <inheritdoc />
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Update();
        }

        private void Update()
        {
            var image = _image;
            if (image == null)
                return;

            var sources = Sources;
            if (sources == null)
                return;

            var rect = new Rect(0, 0, ActualWidth, ActualHeight);
            var viewport = Viewport ?? Window.GetWindow(this);
            if (viewport == null)
                return;

            var visualTransform = image.TransformToVisual(viewport);
            var extent = visualTransform.TransformBounds(rect).Size;

            // ReSharper disable once PossibleNullReferenceException
            var imageSources = sources.OrderBy(source => source.Height).ToArray();
            if (!imageSources.Any())
                return;

            var thresholds = Enumerable.Range(0, imageSources.Length - 1)
                // ReSharper disable PossibleNullReferenceException
                .Select(index => imageSources[index].Height + Math.Sqrt(imageSources[index + 1].Height - imageSources[index].Height));
            // ReSharper restore PossibleNullReferenceException

            var imageIndex = thresholds.Count(threshold => threshold <= extent.Height);

            image.Source = imageSources[imageIndex];
        }
    }
}
