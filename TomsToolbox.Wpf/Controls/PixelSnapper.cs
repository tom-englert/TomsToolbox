namespace TomsToolbox.Wpf.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Moves the view port by fractional pixels, so the control's top left point is always aligned on a pixel boundary.
    /// See e.g. <see href="https://msdn.microsoft.com/de-de/library/aa970908.aspx"/>.
    /// </summary>
    public class PixelSnapper : ContentControl, ILayer
    {
        private static readonly Point ZeroPoint = new Point(0.0, 0.0);
        private Size _physicalPixelSize = new Size(1.0, 1.0);

        /// <summary>
        /// Initializes a new instance of the <see cref="PixelSnapper"/> class.
        /// </summary>
        public PixelSnapper()
        {
            Loaded += (_, __) => _physicalPixelSize = this.GetPhysicalPixelSize();
        }

        /// <summary>
        /// Gets or sets the view port displaying this control.
        /// </summary>
        public FrameworkElement Viewport
        {
            get { return (FrameworkElement)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="Viewport"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof(FrameworkElement), typeof(PixelSnapper));

        /// <summary>
        /// Invalidates the layout of this instance.
        /// </summary>
        public void Invalidate()
        {
            var viewPort = Viewport;
            if (viewPort == null)
                return;

            var p = ZeroPoint.Translate(this, viewPort);

            viewPort.RenderTransform = new TranslateTransform(-(p.X  % _physicalPixelSize.Width), -(p.Y % _physicalPixelSize.Height));
        }
    }
}
