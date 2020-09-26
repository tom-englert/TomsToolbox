namespace TomsToolbox.Wpf
{
    /// <summary>
    /// Interface to be implemented by visual layers that need to be forced to invalidate their layout independent of the WPF render cycle.
    /// </summary>
    interface ILayer
    {
        /// <summary>
        /// Invalidates the layout of this instance.
        /// </summary>
        void Invalidate();
    }
}
