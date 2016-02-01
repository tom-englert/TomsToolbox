namespace TomsToolbox.Wpf.Behaviors
{
    using System.Diagnostics.Contracts;
    using System.Windows.Interactivity;

    using TomsToolbox.Wpf.Controls;

    public class MapTouchBehavior : Behavior<Map>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            var map = AssociatedObject;
            Contract.Assume(map != null);

            map.ManipulationDelta += Map_ManipulationDelta;
        }

        private void Map_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var map = AssociatedObject;
            Contract.Assume(map != null);

            map.Center += e.DeltaManipulation.Translation;
        }
    }
}
