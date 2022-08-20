namespace TomsToolbox.Wpf;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using TomsToolbox.Essentials;

/// <summary>
/// Extensions for multi selectors like ListBox or DataGrid:
/// <list type="bullet">
/// <item>Support binding operations with SelectedItems property.</item>
/// </list>
/// </summary>
/// <remarks>
/// SelectionBinding:
/// <para/>
/// Since there is no common interface for ListBox and DataGrid, the SelectionBinding is implemented via reflection/dynamics, so it will
/// work on any FrameworkElement that has the SelectedItems, SelectedItem and SelectedItemIndex properties and the SelectionChanged event.
/// </remarks>
public static class MultiSelectorExtensions
{
#pragma warning disable CA1825 // Avoid zero-length array allocations (=> Net45)
    private static readonly IList _emptyObjectArray = new object[0];

    /// <summary>
    /// Gets the value of the <see cref="P:TomsToolbox.Wpf.MultiSelectorExtensions.SelectionBinding"/> attached property.
    /// </summary>
    /// <param name="obj">The object to attach to.</param>
    /// <returns>The current selection.</returns>
    [AttachedPropertyBrowsableForType(typeof(Selector))]
    public static IList? GetSelectionBinding(this Selector obj)
    {
        return (IList?)obj.GetValue(SelectionBindingProperty);
    }
    /// <summary>
    /// Sets the value of the <see cref="P:TomsToolbox.Wpf.MultiSelectorExtensions.SelectionBinding"/> attached property.
    /// </summary>
    /// <param name="obj">The object to attach to.</param>
    /// <param name="value">The new selection.</param>
    [AttachedPropertyBrowsableForType(typeof(Selector))]
    public static void SetSelectionBinding(this Selector obj, IList? value)
    {
        obj.SetValue(SelectionBindingProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wpf.MultiSelectorExtensions.SelectionBinding"/> dependency property.
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>
    /// Attach this property to a ListBox or DataGrid to bind the selectors SelectedItems property to the view models SelectedItems property.
    /// </summary>
    /// <example>
    /// If your view model has two properties "AnyList Items { get; }" and "IList SelectedItems { get; set; }" your XAML looks like this:
    /// <para/>
    /// <code><![CDATA[
    /// <ListBox ItemsSource="{Binding Path=Items}" core:MultiSelectorExtensions.SelectionBinding="{Binding Path=SelectedItems}"/>
    /// ]]></code>
    /// </example>
    /// </AttachedPropertyComments>
    public static readonly DependencyProperty SelectionBindingProperty =
        DependencyProperty.RegisterAttached("SelectionBinding", typeof(IList), typeof(MultiSelectorExtensions), new FrameworkPropertyMetadata(null, SelectionBinding_Changed));
    private static readonly DependencyProperty SelectionSynchronizerProperty =
        DependencyProperty.RegisterAttached("SelectionSynchronizer", typeof(SelectionSynchronizer), typeof(MultiSelectorExtensions));

    private static void SelectionBinding_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // The selector is the target of the binding, and the ViewModel property is the source.
        var synchronizer = (SelectionSynchronizer?)d.GetValue(SelectionSynchronizerProperty);

        if (synchronizer != null)
        {
            if (synchronizer.IsUpdating)
                return;

            synchronizer.Dispose();
        }

        if (e.NewValue is not IList sourceSelection)
            return;

        d.SetValue(SelectionSynchronizerProperty, new SelectionSynchronizer((Selector)d, sourceSelection));
    }

    private static void CommitEdit(this Selector? selector)
    {
        if (selector is DataGrid dataGrid)
        {
            dataGrid.CommitEdit(); // cell
            dataGrid.CommitEdit(); // row
        }
    }
    private static IList GetSelectedItems(this Selector selector)
    {
        var selectedItems = (IList)((dynamic)selector).SelectedItems;
        return selectedItems;
    }
    private static void ScrollIntoView(this Selector selector, object? selectedItem)
    {
        ((dynamic)selector).ScrollIntoView(selectedItem);
    }

    private static void BeginSetFocus(this ItemsControl selector, object? selectedItem)
    {
        selector.BeginInvoke(() =>
        {
            if (selector.ItemContainerGenerator.ContainerFromItem(selectedItem) is not FrameworkElement container)
                return;

            var child = container.VisualDescendantsAndSelf().OfType<UIElement>().FirstOrDefault(item => item.Focusable);

            child?.Focus();
        });
    }

    private static void ClearSourceSelection(this Selector selector)
    {
        var sourceSelection = selector.GetSelectionBinding();

        if (sourceSelection == null)
            return;

        if (sourceSelection.IsFixedSize || sourceSelection.IsReadOnly)
        {
            selector.SetSelectionBinding(_emptyObjectArray);
        }
        else
        {
            sourceSelection.Clear();
        }
    }

    private static void SynchronizeWithSource(this Selector selector, IList sourceSelection)
    {
        var selectedItems = selector.GetSelectedItems();

        if ((selectedItems.Count == sourceSelection.Count) && sourceSelection.Cast<object>().All(selectedItems.Contains))
            return;

        selector.CommitEdit();

        // Clear the selection.
        selector.SelectedIndex = -1;

        if (sourceSelection.Count == 1)
        {
            selector.SelectSingleItem(sourceSelection);
        }
        else
        {
            selector.AddItemsToSelection(sourceSelection);
        }
    }

    private static void AddItemsToSelection(this Selector selector, IList itemsToSelect)
    {
        var isSourceInvalid = false;
        var selectedItems = selector.GetSelectedItems();
        var itemsToRemove = new List<object?>();

        foreach (var item in itemsToSelect)
        {
            if (selector.Items.Contains(item))
            {
                selectedItems.Add(item);
            }
            else
            {
                // The item is not present, e.g. because of filtering, and can't be selected at this time.
                if (itemsToSelect.IsFixedSize || itemsToSelect.IsReadOnly)
                {
                    isSourceInvalid = true;
                }
                else
                {
                    itemsToRemove.Add(item);
                }
            }
        }

        if (isSourceInvalid)
        {
            selector.SetSelectionBinding(ArrayCopy(selector.GetSelectedItems()));
        }
        else
        {
            itemsToRemove.ForEach(itemsToSelect.Remove);
        }
    }

    private static void SelectSingleItem(this Selector selector, IList sourceSelection)
    {
        // Special handling, maybe list box is in single selection mode where we can't call selectedItems.Add().
        var selectedItem = sourceSelection[0];

        // The item is not present, e.g. because of filtering, and can't be selected at this time.
        if (selector.Items.Contains(selectedItem) != true)
        {
            selector.ClearSourceSelection();
        }
        else
        {
            selector.SelectedItem = selectedItem;
            selector.ScrollIntoView(selectedItem);

            if (selector.IsKeyboardFocusWithin)
            {
                selector.BeginSetFocus(selectedItem);
            }
        }
    }

    private static IList ArrayCopy(ICollection source)
    {
        var target = new object[source.Count];
        source.CopyTo(target, 0);
        return target;
    }

    private sealed class SelectionSynchronizer : IDisposable
    {
        private readonly Selector _selector;
        private readonly INotifyCollectionChanged? _observableSourceSelection;

        private readonly bool _selectorHasItemsSourceBinding;

        public SelectionSynchronizer(Selector selector, IList sourceSelection)
        {
            _selector = selector;
            _selectorHasItemsSourceBinding = selector.ItemsSource != null;

            selector.SynchronizeWithSource(sourceSelection);

            selector.SelectionChanged += Selector_SelectionChanged;

            if (sourceSelection.IsFixedSize || sourceSelection.IsReadOnly)
                return;

            _observableSourceSelection = sourceSelection as INotifyCollectionChanged;

            if (_observableSourceSelection != null)
            {
                _observableSourceSelection.CollectionChanged += SourceSelection_CollectionChanged;
            }
        }

        internal bool IsUpdating
        {
            get;
            private set;
        }

        private void SourceSelection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsUpdating || sender == null)
                return;

            IsUpdating = true;

            try
            {
                _selector.CommitEdit();

                var selectedItems = _selector.GetSelectedItems();

                var itemsToSelect = e.NewItems!;
                var itemsToDeselect = e.OldItems!;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        var sourceSelection = (IList)sender;
                        _selector.SynchronizeWithSource(sourceSelection);
                        break;

                    case NotifyCollectionChangedAction.Add:
                        if ((selectedItems.Count == 0) && (itemsToSelect.Count == 1))
                            _selector.SelectSingleItem(itemsToSelect);
                        else
                            _selector.AddItemsToSelection(itemsToSelect);
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        selectedItems.RemoveRange(itemsToDeselect);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        if ((selectedItems.Count == 1) && (itemsToSelect.Count == 1))
                        {
                            _selector.SelectSingleItem(itemsToSelect);
                        }
                        else
                        {
                            selectedItems.RemoveRange(itemsToDeselect);
                            _selector.AddItemsToSelection(itemsToSelect);
                        }
                        break;
                }
            }
            finally
            {
                IsUpdating = false;
            }
        }

        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsUpdating)
                return;

            // SelectionChanged is a routed event, so we might get it from children, too!
            if (e.OriginalSource != sender)
                return;

            // Selector is about to be unloaded - stop synchronizing so we keep the selection in the view model.
            if (_selectorHasItemsSourceBinding && (_selector.ItemsSource == null))
                return;

            IsUpdating = true;

            try
            {
                var sourceSelection = _selector.GetSelectionBinding();
                if (sourceSelection == null)
                    return;

                if (sourceSelection.IsFixedSize || sourceSelection.IsReadOnly)
                {
                    _selector.SetSelectionBinding(ArrayCopy(_selector.GetSelectedItems()));
                }
                else
                {
                    sourceSelection.RemoveRange(e.RemovedItems ?? _emptyObjectArray);
                    sourceSelection.AddRange(e.AddedItems ?? _emptyObjectArray);
                }
            }
            finally
            {
                IsUpdating = false;
            }
        }

        public void Dispose()
        {
            _selector.SelectionChanged -= Selector_SelectionChanged;

            if (_observableSourceSelection != null)
            {
                _observableSourceSelection.CollectionChanged -= SourceSelection_CollectionChanged;
            }
        }
    }
}