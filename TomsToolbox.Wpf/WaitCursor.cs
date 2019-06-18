namespace TomsToolbox.Wpf
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper class to ease automatic display of the wait cursor.
    /// </summary>
    public static class WaitCursor
    {
        /// <summary>
        /// Sets the cursor property of the framework element to the "Wait" cursor and
        /// automatically resets the cursor to the default cursor when the dispatcher becomes idle again.
        /// </summary>
        /// <param name="frameworkElement">The element on which to set the cursor.</param>
        public static void StartLocal([NotNull] FrameworkElement frameworkElement)
        {
            StartLocal(frameworkElement, DispatcherPriority.Background);
        }

        /// <summary>
        /// Sets the cursor property of the framework element to the "Wait" cursor and
        /// automatically resets the cursor to the default cursor when the dispatcher becomes idle again.
        /// </summary>
        /// <param name="frameworkElement">The element on which to set the cursor.</param>
        /// <param name="priority">The dispatcher priority used for waiting.</param>
        public static void StartLocal([NotNull] FrameworkElement frameworkElement, DispatcherPriority priority)
        {
            if (frameworkElement.Cursor == Cursors.Wait)
                return;

            frameworkElement.Cursor = Cursors.Wait;
            // Wait until the WM_CURSOR message has been processed and the cursor is visible:
            frameworkElement.ProcessMessages();
            frameworkElement.BeginInvoke(priority, () => frameworkElement.Cursor = null);
        }

        /// <summary>
        /// Sets the cursor property of the framework elements root visual to the "Wait" cursor and
        /// automatically resets the cursor to the default cursor when the dispatcher becomes idle again.
        /// </summary>
        /// <param name="frameworkElement">An element in the visual tree to start looking for the root visual.</param>
        /// <remarks>
        /// The root visual usually is the whole window, except for controls embedded in native or WindowsForms windows.
        /// </remarks>
        public static void Start([NotNull] FrameworkElement frameworkElement)
        {
            Start(frameworkElement, DispatcherPriority.Background);
        }

        /// <summary>
        /// Sets the cursor property of the framework elements root visual to the "Wait" cursor and
        /// automatically resets the cursor to the default cursor when the dispatcher becomes idle again.
        /// </summary>
        /// <param name="frameworkElement">An element in the visual tree to start looking for the root visual.</param>
        /// <param name="priority">The dispatcher priority used for waiting.</param>
        /// <remarks>
        /// The root visual usually is the whole window, except for controls embedded in native or WindowsForms windows.
        /// </remarks>
        public static void Start([NotNull] FrameworkElement frameworkElement, DispatcherPriority priority)
        {
            StartLocal(frameworkElement.GetRootVisual(), priority);
        }
    }
}