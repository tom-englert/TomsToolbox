namespace TomsToolbox.Wpf.Interactivity
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Animation;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Controls;

    /// <summary>
    /// Implements pan behavior for the <see cref="Map"/> control.
    /// </summary>
    public class MapPanBehavior : FrameworkElementBehavior<Map>
    {
        [NotNull]
        private readonly PointAnimation _panAnimation = new PointAnimation { Duration = new Duration(TimeSpan.FromSeconds(0.25)) };
        [NotNull]
        private readonly Storyboard _storyboard = new Storyboard();

        private Point? _panPosition;
        private bool _isStoryboardRunning;

        [NotNull]
        private static readonly DependencyProperty AnimatedPanPositionProperty =
            DependencyProperty.Register("AnimatedPanPosition", typeof(Point), typeof(MapPanBehavior), new FrameworkPropertyMetadata((sender, e) => ((MapPanBehavior)sender)?.AnimatedPanPosition_Changed((Point)e.NewValue)));

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the Freezable derived class. 
        /// </summary>
        /// <returns>The new instance.</returns>
        protected override Freezable CreateInstanceCore()
        {
            // ensure we don't return a copy on freeze, else the animation is gone.
            return this;
        }

        private void AnimatedPanPosition_Changed(Point newValue)
        {
            var map = AssociatedObject;
            if (map == null)
                return;

            map.Center = newValue;
        }

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

            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            map.MouseMove += Map_MouseMove;

            Storyboard.SetTarget(_panAnimation, this);
            Storyboard.SetTargetProperty(_panAnimation, new PropertyPath(AnimatedPanPositionProperty));
            _storyboard.Children?.Add(_panAnimation);
            _storyboard.Completed += Storyboard_Completed;
        }

        /// <inheritdoc />
        protected override void OnAssociatedObjectLoaded()
        {
            base.OnAssociatedObjectLoaded();

            var map = AssociatedObject;
            var focusableParent = map.AncestorsAndSelf().OfType<FrameworkElement>().FirstOrDefault(item => item.Focusable);
            if (focusableParent != null)
            {
                focusableParent.KeyDown += FocusableParent_KeyDown;
            }
        }

        private void FocusableParent_KeyDown([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            switch (e.Key)
            {
                case Key.Left:
                    Pan(-1, 0);
                    break;

                case Key.Right:
                    Pan(+1, 0);
                    break;

                case Key.Up:
                    Pan(0, -1);
                    break;

                case Key.Down:
                    Pan(0, +1);
                    break;

                default:
                    return;
            }

            e.Handled = true;
        }

        private void Pan(double dx, double dy)
        {
            Pan(new Vector(dx / 4, dy / 4));
        }

        private void Pan(Vector delta)
        {
            if (_isStoryboardRunning)
                return;

            var map = AssociatedObject;
            if (map == null)
                return;

            _panAnimation.From = map.Center;
            _panAnimation.To = map.Center - delta / map.ZoomFactor;
            _isStoryboardRunning = true;
            _storyboard.Begin();
        }

        private void Storyboard_Completed([CanBeNull] object? sender, [NotNull] EventArgs e)
        {
            _isStoryboardRunning = false;
        }

        private void Map_MouseMove([NotNull] object sender, [NotNull] MouseEventArgs e)
        {
            if (_panPosition == null)
                return;

            var map = AssociatedObject;

            var layer = map?.World;
            if (layer == null)
                return;

            var mousePosition = e.GetPosition(layer);

            map!.Center += _panPosition.GetValueOrDefault() - mousePosition;
        }

        private void Map_MouseLeftButtonDown([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            var map = AssociatedObject;

            var layer = map?.World;
            if (layer == null)
                return;

            _panPosition = e.GetPosition(layer);

            map!.CaptureMouse();
        }

        private void Map_MouseLeftButtonUp([NotNull] object sender, [NotNull] MouseButtonEventArgs e)
        {
            _panPosition = null;

            AssociatedObject?.ReleaseMouseCapture();
        }
    }
}
