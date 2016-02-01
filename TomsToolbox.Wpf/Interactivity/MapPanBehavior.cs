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
    using TomsToolbox.Wpf.Controls;

    /// <summary>
    /// Implements pan behavior for the <see cref="Map"/> control.
    /// </summary>
    public class MapPanBehavior : Behavior<Map>
    {
        private readonly PointAnimation _panAnimation = new PointAnimation { Duration = new Duration(TimeSpan.FromSeconds(0.25)) };
        private readonly Storyboard _storyboard = new Storyboard();

        private Point? _panPosition;
        private bool _isStoryboardRunning;

        private static readonly DependencyProperty AnimatedPanPositionProperty =
            DependencyProperty.Register("AnimatedPanPosition", typeof(Point), typeof(MapPanBehavior), new FrameworkPropertyMetadata((sender, e) => ((MapPanBehavior)sender).AnimatedPanPosition_Changed((Point)e.NewValue)));

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
            Contract.Assume(map != null);

            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            map.MouseMove += Map_MouseMove;

            var focusableParent = map.AncestorsAndSelf().OfType<FrameworkElement>().FirstOrDefault(item => item.Focusable);
            if (focusableParent != null)
            {
                focusableParent.KeyDown += FocusableParent_KeyDown;
            }

            Storyboard.SetTarget(_panAnimation, this);
            Storyboard.SetTargetProperty(_panAnimation, new PropertyPath(AnimatedPanPositionProperty));
            _storyboard.Children.Maybe().Do(c => c.Add(_panAnimation));
            _storyboard.Completed += Storyboard_Completed;

            CommandManager.RegisterClassCommandBinding(typeof(Map), new CommandBinding(ComponentCommands.MoveLeft, (_, __) => Pan(1, 0)));
            CommandManager.RegisterClassCommandBinding(typeof(Map), new CommandBinding(ComponentCommands.MoveRight, (_, __) => Pan(-1, 0)));
            CommandManager.RegisterClassCommandBinding(typeof(Map), new CommandBinding(ComponentCommands.MoveUp, (_, __) => Pan(0, 1)));
            CommandManager.RegisterClassCommandBinding(typeof(Map), new CommandBinding(ComponentCommands.MoveDown, (_, __) => Pan(0, -1)));
        }

        void FocusableParent_KeyDown(object sender, KeyEventArgs e)
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
            }
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

        void Storyboard_Completed(object sender, EventArgs e)
        {
            _isStoryboardRunning = false;
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (_panPosition == null)
                return;

            var map = AssociatedObject;
            if (map == null)
                return;

            var layer = map.World;
            if (layer == null)
                return;

            var mousePosition = e.GetPosition(layer);

            map.Center += _panPosition.GetValueOrDefault() - mousePosition;
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var map = AssociatedObject;
            if (map == null)
                return;

            var layer = map.World;
            if (layer == null)
                return;

            _panPosition = e.GetPosition(layer);

            map.CaptureMouse();
        }

        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _panPosition = null;

            var map = AssociatedObject;
            if (map == null)
                return;

            map.ReleaseMouseCapture();
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_panAnimation != null);
            Contract.Invariant(_storyboard != null);
        }
    }
}
