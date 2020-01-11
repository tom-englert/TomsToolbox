namespace TomsToolbox.Wpf.Interactivity
{
    using JetBrains.Annotations;

    using Microsoft.Xaml.Behaviors;

    using TomsToolbox.Wpf.Controls;

    /// <summary>
    /// Behavior to add support for touch manipulation to the <see cref="Map"/> object.
    /// </summary>
    public class MapTouchBehavior : Behavior<Map>
    {
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

            map.ManipulationDelta += Map_ManipulationDelta;
        }

        private void Map_ManipulationDelta([CanBeNull] object? sender, [NotNull] System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var map = AssociatedObject;

            map.Center += e.DeltaManipulation.Translation;
        }
    }
}
