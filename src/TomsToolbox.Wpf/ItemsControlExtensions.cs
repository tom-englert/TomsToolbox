namespace TomsToolbox.Wpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Extensions and helpers for the <see cref="ItemsControl"/> or derived classes.
/// </summary>
public static class ItemsControlExtensions
{
    private static readonly TimeSpan _doubleClickTime = TimeSpan.FromMilliseconds(NativeMethods.GetDoubleClickTime());
    private static DateTime _lastClickHandled;

    /// <summary>
    /// Gets the default item command. See <see cref="P:TomsToolbox.Wpf.ItemsControlExtensions.DefaultItemCommand"/> attached property for details.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The command.</returns>
    [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
    public static ICommand? GetDefaultItemCommand(this ItemsControl obj)
    {
        return (ICommand?)obj.GetValue(DefaultItemCommandProperty);
    }
    /// <summary>
    /// Sets the default item command. See <see cref="P:TomsToolbox.Wpf.ItemsControlExtensions.DefaultItemCommand"/> attached property for details.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="value">The command.</param>
    [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
    public static void SetDefaultItemCommand(this ItemsControl obj, ICommand? value)
    {
        obj.SetValue(DefaultItemCommandProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wpf.ItemsControlExtensions.DefaultItemCommand"/> attached property
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>
    /// The default item command is the command that will be executed when an item of the items control has received a mouse double click or enter key.
    /// It is not executed when the double-click is on the background or on the scrollbar.
    /// This command avoids the ubiquitous wrong implementations as well as code duplication when handling double-clicks in items controls like the <see cref="ListBox"/>
    /// <para/>
    /// The command parameter for the command is the item that has been clicked.
    /// </summary>
    /// </AttachedPropertyComments>
    public static readonly DependencyProperty DefaultItemCommandProperty =
        DependencyProperty.RegisterAttached("DefaultItemCommand", typeof(ICommand), typeof(ItemsControlExtensions), new FrameworkPropertyMetadata(DefaultItemCommand_Changed));

    private static void DefaultItemCommand_Changed(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is ItemsControl itemsControl))
            return;

        itemsControl.MouseDoubleClick -= ItemsControl_MouseDoubleClick;
        itemsControl.KeyDown -= ItemsControl_KeyDown;

        if (e.NewValue == null)
            return;

        itemsControl.MouseDoubleClick += ItemsControl_MouseDoubleClick;
        itemsControl.KeyDown += ItemsControl_KeyDown;
    }

    static void ItemsControl_KeyDown(object sender, KeyEventArgs e)
    {
        if ((e.Key != Key.Enter) || e.Handled)
            return;

        ExecuteCommand(sender, e);
    }

    static void ItemsControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.Handled)
            return;

        ExecuteCommand(sender, e);
    }

    private static void ExecuteCommand(object sender, RoutedEventArgs e)
    {
        if (DateTime.Now < (_lastClickHandled + _doubleClickTime))
            return; // avoid duplicate actions on nested controls, EVERY items control will receive the double click event.

        if (!(sender is ItemsControl itemsControl))
            return;

        if (!(e.OriginalSource is DependencyObject originalSource))
            return;

        var command = GetDefaultItemCommand(itemsControl);
        if (command == null)
            return;

        // Bubble up until we find the ItemContainer and it's associated item.
        foreach (var element in originalSource.AncestorsAndSelf())
        {
            if (ReferenceEquals(element, itemsControl))
                return; // we have clicked somewhere else, but not on the ItemContainer.

            if (element is GroupItem)
                return;

            var item = itemsControl.ItemContainerGenerator.ItemFromContainer(element);

            if ((item == null) || (item == DependencyProperty.UnsetValue))
                continue;

            if (command.CanExecute(item))
                command.Execute(item);

            e.Handled = true;
            _lastClickHandled = DateTime.Now;

            return;
        }
    }

    /// <summary>
    /// Gets the object that will be observed for changes.
    /// A change of the object will trigger a refresh on the collection view of the attached items control.
    /// </summary>
    /// <param name="obj">The <see cref="ItemsControl"/> to refresh.</param>
    /// <returns>The object to observe.</returns>
    public static object? GetRefreshOnSourceChanges(ItemsControl obj)
    {
        return obj.GetValue(RefreshOnSourceChangesProperty);
    }
    /// <summary>
    /// Sets the object that will be observed for changes.
    /// A change of the object will trigger a refresh on the collection view of the attached items control.
    /// </summary>
    /// <param name="obj">The <see cref="ItemsControl"/> to refresh.</param>
    /// <param name="value">The object to observe.</param>
    public static void SetRefreshOnSourceChanges(ItemsControl obj, object? value)
    {
        obj.SetValue(RefreshOnSourceChangesProperty, value);
    }
    /// <summary>
    /// Identifies the <see cref="P:TomsToolbox.Wpf.ItemsControlExtensions.RefreshOnSourceChanges"/> attached property
    /// </summary>
    /// <AttachedPropertyComments>
    /// <summary>
    /// The object that will be observed for changes. A change of the object will trigger a refresh on the collection view of the attached items control.
    /// </summary>
    /// </AttachedPropertyComments>
    public static readonly DependencyProperty RefreshOnSourceChangesProperty =
        DependencyProperty.RegisterAttached("RefreshOnSourceChanges", typeof(object), typeof(ItemsControlExtensions), new FrameworkPropertyMetadata(Source_Changed));

    private static void Source_Changed(DependencyObject? d, DependencyPropertyChangedEventArgs e)
    {
        if (!(d is ItemsControl itemsControl))
            return;

        // Collection views are maybe nested! Must recurse through all views, the top one might not own the filter!
        ICollectionView? itemCollection = itemsControl.Items;
        while (itemCollection != null)
        {
            itemCollection.Refresh();

            var sourceCollection = itemCollection.SourceCollection as ICollectionView;
            if (itemCollection == sourceCollection)
                break;

            itemCollection = sourceCollection;
        }
    }


    /// <summary>
    /// Gets the item containers for a items control.
    /// </summary>
    /// <typeparam name="T">The type of the containers.</typeparam>
    /// <param name="itemsControl">The items control.</param>
    /// <returns>The list of containers; contains <c>null</c> entries for unrealized containers (see <see cref="ItemContainerGenerator.ContainerFromIndex"/>).</returns>
    public static IEnumerable<T?> GetItemContainers<T>(this ItemsControl itemsControl)
        where T : DependencyObject
    {
        var generator = itemsControl.ItemContainerGenerator;

        var itemsCount = Math.Max(0, itemsControl.Items.Count);

        return Enumerable.Range(0, itemsCount)
            .Select(i => generator.ContainerFromIndex(i) as T);
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int GetDoubleClickTime();
    }
}