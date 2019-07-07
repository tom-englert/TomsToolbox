namespace TomsToolbox.Wpf.Composition
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;

    using TomsToolbox.Wpf.Composition.XamlExtensions;

    /// <inheritdoc />
    /// <summary>
    /// Retrieves all exported items with the  <see cref="T:TomsToolbox.Wpf.Composition.VisualCompositionExportAttribute" /> that match the RegionId from the composition container and assigns them as the items source of the associated <see cref="T:System.Windows.Controls.ItemsControl" />
    /// <para />
    /// If the items control is a <see cref="T:System.Windows.Controls.Primitives.Selector" />, and the composable object implement <see cref="T:TomsToolbox.Wpf.Composition.ISelectableComposablePart" />, the selection of the selector is synchronized with the <see cref="P:TomsToolbox.Wpf.Composition.ISelectableComposablePart.IsSelected" /> property.
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

        /// <inheritdoc />
        /// <summary>
        /// Updates this instance.
        /// </summary>
        protected override void OnUpdate()
        {
            var regionId = RegionId;
            var itemsControl = AssociatedObject;
            var selector = itemsControl as Selector;

            VisualComposition.OnTrace(this, $"Update {GetType()}, RegionId={regionId}, ItemsControl={itemsControl}");

            if (itemsControl == null)
                return;

            if (string.IsNullOrEmpty(regionId))
            {
                itemsControl.ItemsSource = null;
                return;
            }

            var exports = GetExports(regionId);
            if (exports == null)
            {
                VisualComposition.OnTrace(this, $"Update {GetType()}: No exports for RegionId={regionId} found");
                return;
            }

            var exportedItems = exports
                .OrderBy(item => item.Metadata?.Sequence)
                .Select(item => GetTarget(item.Value))
                .ToArray();

            VisualComposition.OnTrace(this, $"Update {GetType()}, Found {exportedItems.Length} items");

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
            foreach (var item in composables.OfType<IComposablePartWithContext>())
            {
                // ReSharper disable once PossibleNullReferenceException
                item.CompositionContext = context;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            var itemsControl = AssociatedObject;

            if (!(itemsControl is Selector))
                return;

            var items = itemsControl.ItemsSource;
            if (items == null)
                return;

            DetachSelectables(items);

            base.OnDetaching();
        }

        private void AttachSelectables([NotNull, ItemCanBeNull] IEnumerable viewModels)
        {
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
            if (!(AssociatedObject is Selector selector))
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
