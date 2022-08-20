namespace TomsToolbox.Wpf.Composition;

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using TomsToolbox.Wpf.Composition.XamlExtensions;

/// <inheritdoc />
/// <summary>
/// Retrieves all exported items with the  <see cref="T:TomsToolbox.Wpf.Composition.AttributedModel.VisualCompositionExportAttribute" /> that match the RegionId from the composition container and assigns them as the items source of the associated <see cref="T:System.Windows.Controls.ItemsControl" />
/// <para />
/// If the items control is a <see cref="T:System.Windows.Controls.Primitives.Selector" />, and the composable object implement <see cref="T:TomsToolbox.Wpf.Composition.ISelectableComposablePart" />, the selection of the selector is synchronized with the <see cref="P:TomsToolbox.Wpf.Composition.ISelectableComposablePart.IsSelected" /> property.
/// </summary>
public class ItemsControlCompositionBehavior : VisualCompositionBehavior<ItemsControl>
{
    /// <summary>
    /// Gets or sets a value indicating whether the behavior will force the selection of the first element after applying the content.<para/>
    /// This will ensure that e.g. a tab-control will show the first tab instead of empty content.<para/>
    /// The default value is <c>true</c>.
    /// </summary>
    public bool ForceSelection { get; set; } = true;

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

            if (ForceSelection && (selector.SelectedIndex == -1) && !selector.Items.IsEmpty)
            {
                selector.SelectedIndex = 0;
            }
        }
    }

    private static void ApplyContext(IEnumerable composables, object? context)
    {
        foreach (var item in composables.OfType<IComposablePartWithContext>())
        {
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

    private void AttachSelectables(IEnumerable viewModels)
    {
        var selectables = viewModels.OfType<ISelectableComposablePart>();

        foreach (var selectable in selectables)
        {
            selectable.PropertyChanged += Selectable_PropertyChanged;
        }
    }

    private void DetachSelectables(IEnumerable? viewModels)
    {
        if (viewModels == null)
            return;

        var selectables = viewModels.OfType<ISelectableComposablePart>();

        foreach (var selectable in selectables)
        {
            selectable.PropertyChanged -= Selectable_PropertyChanged;
        }
    }

    private void Selectable_PropertyChanged(object? sender, PropertyChangedEventArgs? e)
    {
        if (!(AssociatedObject is Selector selector))
            return;

        if (sender is not ISelectableComposablePart selectable)
            return;

        if (string.Equals(e?.PropertyName, "IsSelected", StringComparison.Ordinal) && selectable.IsSelected)
        {
            selector.SelectedItem = selectable;
        }
    }

    private static void Selector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
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