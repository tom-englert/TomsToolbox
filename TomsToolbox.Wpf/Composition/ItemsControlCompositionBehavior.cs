namespace TomsToolbox.Wpf.Composition
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Retrieves all exported items with the  <see cref="VisualCompositionExportAttribute"/> that match the RegionId from the composition container and assigns them as the items source of the associated <see cref="ItemsControl"/>
    /// <para/>
    /// If the items control is a <see cref="Selector"/>, and the composable object implement <see cref="ISelectableComposablePart"/>, the selection of the selector is synchronized with the <see cref="ISelectableComposablePart.IsSelected"/> property.
    /// </summary>
    public class ItemsControlCompositionBehavior : VisualCompositionBehavior<ItemsControl>
    {
        private bool _forceSelection = true;

        /// <summary>
        /// Gets or sets a value indicating whether the behavior will force the selection of the first element after applying the content.<para/>
        /// This will ensure that e.g. a tab-control will show the first tab instead of empty content.<para/>
        /// The default value is <c>true</c>.
        /// </summary>
        public bool ForceSelection
        {
            get
            {
                return _forceSelection;
            }
            set
            {
                _forceSelection = value;
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        protected override void OnUpdate()
        {
            var regionId = RegionId;
            var itemsControl = AssociatedObject;
            var selector = itemsControl as Selector;

            if (itemsControl == null)
                return;

            if (string.IsNullOrEmpty(regionId))
            {
                itemsControl.ItemsSource = null;
                return;
            }

            var exports = GetExports(regionId);
            if (exports == null)
                return;

            var exportedItems = exports
                .OrderBy(item => item.Metadata.Sequence)
                .Select(item => GetTarget(item.Value))
                .ToArray();

            var currentItems = itemsControl.Items.Cast<object>().ToArray();

            if (exportedItems.SequenceEqual(currentItems))
            {
                ApplyContext(currentItems, CompositionContext);
                return;
            }

            if (selector != null)
            {
                selector.SelectionChanged -= Selector_SelectionChanged;
                DetachSelectables(currentItems);
            }

            ApplyContext(currentItems.Except(exportedItems), null);

            itemsControl.ItemsSource = exportedItems;

            ApplyContext(exportedItems, CompositionContext);

            if (selector != null)
            {
                AttachSelectables(exportedItems);
                selector.SelectionChanged += Selector_SelectionChanged;

                if (_forceSelection && (selector.SelectedIndex == -1) && !selector.Items.IsEmpty)
                {
                    selector.SelectedIndex = 0;
                }
            }
        }

        private static void ApplyContext(IEnumerable composables, object context)
        {
            Contract.Requires(composables != null);

            foreach (var item in composables.OfType<IComposablePartWithContext>())
            {
                item.CompositionContext = context;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            var itemsControl = AssociatedObject;

            var selector = itemsControl as Selector;
            if (selector == null)
                return;

            var items = itemsControl.ItemsSource;
            if (items == null)
                return;

            DetachSelectables(items);

            base.OnDetaching();
        }

        private void AttachSelectables(IEnumerable viewModels)
        {
            Contract.Requires(viewModels != null);

            var selectables = viewModels.OfType<ISelectableComposablePart>();

            foreach (var selectable in selectables)
            {
                selectable.PropertyChanged += Selectable_PropertyChanged;
            }
        }

        private void DetachSelectables(IEnumerable viewModels)
        {
            if (viewModels == null)
                return;

            var selectables = viewModels.OfType<ISelectableComposablePart>();

            foreach (var selectable in selectables)
            {
                selectable.PropertyChanged -= Selectable_PropertyChanged;
            }
        }

        private void Selectable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var selector = AssociatedObject as Selector;
            if (selector == null)
                return;

            var selectable = (ISelectableComposablePart)sender;
            if (selectable == null)
                return;

            if ((e.PropertyName == "IsSelected") && selectable.IsSelected)
            {
                selector.SelectedItem = selectable;
            }
        }

        private static void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems != null)
            {
                foreach (var selectable in e.RemovedItems.OfType<ISelectableComposablePart>())
                {
                    selectable.IsSelected = false;
                }
            }

            if (e.AddedItems != null)
            {
                foreach (var selectable in e.AddedItems.OfType<ISelectableComposablePart>())
                {
                    selectable.IsSelected = true;
                }
            }
        }
    }
}
