namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;

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
            get => _forceSelection;
            set => _forceSelection = value;
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
                .OrderBy(item => item.Metadata?.Sequence)
                .Select(item => GetTarget(item?.Value))
                .ToArray();

            // ReSharper disable once AssignNullToNotNullAttribute
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

                // ReSharper disable once PossibleNullReferenceException
                if (_forceSelection && (selector.SelectedIndex == -1) && !selector.Items.IsEmpty)
                {
                    selector.SelectedIndex = 0;
                }
            }
        }

        private static void ApplyContext([NotNull, ItemCanBeNull] IEnumerable composables, [CanBeNull] object context)
        {
            Contract.Requires(composables != null);

            foreach (var item in composables.OfType<IComposablePartWithContext>())
            {
                // ReSharper disable once PossibleNullReferenceException
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

        private void AttachSelectables([NotNull, ItemCanBeNull] IEnumerable viewModels)
        {
            Contract.Requires(viewModels != null);

            var selectables = viewModels.OfType<ISelectableComposablePart>();

            foreach (var selectable in selectables)
            {
                // ReSharper disable once PossibleNullReferenceException
                selectable.PropertyChanged += Selectable_PropertyChanged;
            }
        }

        private void DetachSelectables([CanBeNull, ItemCanBeNull] IEnumerable viewModels)
        {
            if (viewModels == null)
                return;

            var selectables = viewModels.OfType<ISelectableComposablePart>();

            foreach (var selectable in selectables)
            {
                // ReSharper disable once PossibleNullReferenceException
                selectable.PropertyChanged -= Selectable_PropertyChanged;
            }
        }

        private void Selectable_PropertyChanged([CanBeNull] object sender, [CanBeNull] PropertyChangedEventArgs e)
        {
            var selector = AssociatedObject as Selector;
            if (selector == null)
                return;

            var selectable = (ISelectableComposablePart)sender;
            if (selectable == null)
                return;

            // ReSharper disable once PossibleNullReferenceException
            if (string.Equals(e.PropertyName, "IsSelected", StringComparison.Ordinal) && selectable.IsSelected)
            {
                selector.SelectedItem = selectable;
            }
        }

        private static void Selector_SelectionChanged([CanBeNull] object sender, [NotNull] SelectionChangedEventArgs e)
        {
            if (e.RemovedItems != null)
            {
                foreach (var selectable in e.RemovedItems.OfType<ISelectableComposablePart>())
                {
                    // ReSharper disable once PossibleNullReferenceException
                    selectable.IsSelected = false;
                }
            }

            if (e.AddedItems != null)
            {
                foreach (var selectable in e.AddedItems.OfType<ISelectableComposablePart>())
                {
                    // ReSharper disable once PossibleNullReferenceException
                    selectable.IsSelected = true;
                }
            }
        }
    }
}
