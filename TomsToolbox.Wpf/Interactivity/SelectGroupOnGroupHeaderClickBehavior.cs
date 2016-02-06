using System.Diagnostics.Contracts;
namespace TomsToolbox.Wpf.Interactivity
{
    using System.Diagnostics.Contracts;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// If attached to the root visual in the group header template of a selector control, 
    /// all items in the group will be selected when the group header is clicked, 
    /// or added to the current selection when the Ctlr-key is down. 
    ///
    /// </summary>
    public class SelectGroupOnGroupHeaderClickBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            Contract.Assume(AssociatedObject != null);

            AssociatedObject.MouseLeftButtonDown += GroupHeader_OnMouseLeftButtonDown;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            Contract.Assume(AssociatedObject != null);

            AssociatedObject.MouseLeftButtonDown -= GroupHeader_OnMouseLeftButtonDown;
        }

        private static void GroupHeader_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Contract.Requires(sender != null);

            var visual = sender as FrameworkElement;
            if (visual == null)
                return;

            var group = visual.DataContext as CollectionViewGroup;
            if ((group == null) || (group.Items == null))
                return;

            var selector = visual.TryFindAncestor<Selector>();

            if (selector == null)
                return;

            selector.BeginInit();

            try
            {
                var multiSelector = (dynamic)selector;

                var selectedItems = multiSelector.SelectedItems;
                if (selectedItems == null)
                    return;

                if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                {
                    selectedItems.Clear();
                }

                foreach (var item in group.Items)
                {
                    selectedItems.Add(item);
                }
            }
            catch
            {
                // Element did not have a SelectedItems property.
            }
            finally
            {
                selector.EndInit();
            }
        }
    }
}
