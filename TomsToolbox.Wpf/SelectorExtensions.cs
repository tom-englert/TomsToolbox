namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Threading;

    using TomsToolbox.Desktop;

    /// <summary>
    /// Extensions for the <see cref="Selector"/>
    /// </summary>
    public static class SelectorExtensions
    {
        private static readonly WeakKeyIndexer<int> Cache = new WeakKeyIndexer<int>();

        /// <summary>
        /// Gets the value of the <see cref="P:TomsToolbox.Wpf.SelectorExtensions.TrackSelection"/> attached property.
        /// </summary>
        /// <param name="obj">The selector.</param>
        /// <returns><c>true</c> if the selection should be tracked; otherwise <c>false</c>.</returns>
        [AttachedPropertyBrowsableForType(typeof(Selector))]
        public static bool GetTrackSelection(DependencyObject obj)
        {
            Contract.Requires(obj != null);
            return obj.GetValue<bool>(TrackSelectionProperty);
        }
        /// <summary>
        /// Sets the value of the <see cref="P:TomsToolbox.Wpf.SelectorExtensions.TrackSelection" /> attached property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">if set to <c>true</c> if the selection should be tracked; otherwise <c>false</c>.</param>
        [AttachedPropertyBrowsableForType(typeof(Selector))]
        public static void SetTrackSelection(DependencyObject obj, bool value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(TrackSelectionProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.SelectorExtensions.TrackSelection"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// A value indicating whether selection should be tracked or not.
        /// </summary>
        /// <remarks>
        /// When a <see cref="Selector"/> is embedded in a dynamic page, e.g. another selector,
        /// the visual is recreated or reused with another data context whenever the page is displayed.
        /// <para/>
        /// TrackSelection links the selected index with the view model (DataContext) of the selector,
        /// restoring the cached index whenever the same view model is displayed.
        /// If no index is cached, the first item will be selected.
        /// </remarks>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty TrackSelectionProperty =
            DependencyProperty.RegisterAttached("TrackSelection", typeof(bool), typeof(SelectorExtensions), new FrameworkPropertyMetadata(TrackSelection_Changed));

        private static void TrackSelection_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as Selector;
            if (selector == null)
                return;

            selector.SelectionChanged -= Selector_SelectionChanged;
            selector.Loaded -= Selector_Loaded;

            if (false.Equals(e.NewValue))
                return;

            selector.SelectionChanged += Selector_SelectionChanged;
            selector.Loaded += Selector_Loaded;
        }

        static void Selector_Loaded(object sender, RoutedEventArgs e)
        {
            InternalTrackSelection(sender as Selector, true);
        }

        static void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InternalTrackSelection(sender as Selector, false);
        }

        private static void InternalTrackSelection(Selector selector, bool forceSelection)
        {
            if (selector == null)
                return;

            var dataContext = selector.DataContext;
            if (dataContext == null)
                return;

            if (!selector.IsLoaded)
                return;

            if ((selector.SelectedIndex < 0) || forceSelection)
            {
                selector.BeginInvoke(DispatcherPriority.Loaded, () => selector.SelectedIndex = Cache[dataContext]);
            }
            else
            {
                Cache[dataContext] = selector.SelectedIndex;
            }
        }

        private class WeakKeyIndexer<T>
        {
            private int _cleanupCycleCounter;
            private Dictionary<WeakReference, T> _items = new Dictionary<WeakReference, T>();

            public T this[object key]
            {
                get
                {
                    Contract.Requires(key != null);

                    if ((++_cleanupCycleCounter & 0x7F) == 0)
                        Cleanup();

                    var target = _items.FirstOrDefault(item => item.Key.Target == key);

                    return target.Value;
                }
                set
                {
                    if ((++_cleanupCycleCounter & 0x7F) == 0)
                        Cleanup();

                    var innerKey = _items.Keys.FirstOrDefault(i => i.Target == key) ?? new WeakReference(key);

                    _items[innerKey] = value;
                }
            }

            private void Cleanup()
            {
                _items = _items.Where(item => item.Key.IsAlive).ToDictionary(item => item.Key, item => item.Value);
            }

            [ContractInvariantMethod]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_items != null);
            }
        }
    }
}
