namespace TomsToolbox.Wpf.Interactivity
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System.Windows.Media.Animation;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Controls;

    /// <summary>
    /// Implements zooming behavior for the <see cref="Map"/> control.
    /// </summary>
    public class MapZoomBehavior : Behavior<Map>
    {
        private readonly DoubleAnimation _animation = new DoubleAnimation { Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
        private readonly Storyboard _storyboard = new Storyboard();

        /// <summary>
        /// Gets or sets the number of zoom steps performed on one mouse wheel event.
        /// </summary>
        public double MouseWheelIncrement
        {
            get { return this.GetValue<double>(MouseWheelIncrementProperty); }
            set { SetValue(MouseWheelIncrementProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="MouseWheelIncrement"/> dependency property
        /// </summary>
        public static readonly DependencyProperty MouseWheelIncrementProperty =
            DependencyProperty.Register("MouseWheelIncrement", typeof(double), typeof(MapZoomBehavior), new FrameworkPropertyMetadata(1.0));


        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            var map = AssociatedObject;
            Contract.Assume(map != null);

            map.MouseWheel += AssociatedObject_PreviewMouseWheel;

            var focusableParent = map.AncestorsAndSelf().OfType<FrameworkElement>().FirstOrDefault(item => item.Focusable);
            if (focusableParent != null)
            {
                focusableParent.KeyDown += FocusableParent_KeyDown;
            }

            Storyboard.SetTarget(_animation, map);
            Storyboard.SetTargetProperty(_animation, new PropertyPath(Map.ZoomLevelProperty));

            _storyboard.Children.Maybe().Do(c => c.Add(_animation));
            _storyboard.Completed += Storyboard_Completed;

            CommandManager.RegisterClassCommandBinding(typeof(Map), new CommandBinding(NavigationCommands.DecreaseZoom, (_, __) => Zoom(-1)));
            CommandManager.RegisterClassCommandBinding(typeof(Map), new CommandBinding(NavigationCommands.IncreaseZoom, (_, __) => Zoom(+1)));
        }

        void Storyboard_Completed(object sender, EventArgs e)
        {
            var map = AssociatedObject;
            if (map == null)
                return;

            _animation.To = map.ZoomLevel;
        }

        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Zoom(Math.Sign(e.Delta), e);
        }

        private void Zoom(int delta, MouseEventArgs e = null)
        {
            var map = AssociatedObject;
            if (map == null)
                return;

            var layer = map.World;
            if (layer == null)
                return;

            map.ZoomingPoint = (e != null) ? e.GetPosition(layer) : map.Center;

            var from = _animation.To ?? map.ZoomLevel;
            _animation.To = (Math.Round(from / MouseWheelIncrement) + delta) * MouseWheelIncrement;
            _storyboard.Begin();
        }

        void FocusableParent_KeyDown(object sender, KeyEventArgs e)
        {
            var map = AssociatedObject;
            if (map == null)
                return;

            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            int delta;

            switch (e.Key)
            {
                case Key.Up:
                    delta = +1;
                    break;

                case Key.Down:
                    delta = -1;
                    break;

                default:
                    return;
            }

            map.ZoomingPoint = map.Center;
            var from = _animation.To ?? map.ZoomLevel;
            _animation.To = Math.Round(from) + delta;
            _storyboard.Begin();
        }

        /// <summary>
        /// Objects the invariant.
        /// </summary>
        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_animation != null);
            Contract.Invariant(_storyboard != null);
        }
    }
}
