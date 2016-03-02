namespace TomsToolbox.Wpf.Composition
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Retrieves the exported object that matches RegionId and Role from the composition container
    /// and assigns it as the content of the associated <see cref="ContentControl"/>
    /// </summary>
    public class ContentControlCompositionBehavior : VisualCompositionBehavior<ContentControl>
    {
        /// <summary>
        /// Gets or sets the name of the item that should be attached.
        /// </summary>
        /// <remarks>
        /// The first exported item matching RegionId and Role will be set as the content of the content control.
        /// </remarks>
        public object Role
        {
            get { return GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="Role"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(object), typeof(ContentControlCompositionBehavior), new FrameworkPropertyMetadata((sender, e) => ((ContentControlCompositionBehavior)sender).Role_Changed()));

        private void Role_Changed()
        {
            Update();
        }

        /// <summary>
        /// Updates this instance when any of the constraints have changed.
        /// </summary>
        protected override void Update()
        {
            var regionId = RegionId;
            var role = Role;
            var contentControl = AssociatedObject;

            if (contentControl == null)
                return;

            object exportedItem = null;

            if (!string.IsNullOrEmpty(regionId))
            {
                var exportProvider = ExportProvider;
                var exports = exportProvider.GetExports<IComposablePart, IVisualCompositionMetadata>();

                exportedItem = exports
                    .Where(item => DataTemplateManager.RoleEquals(item.Metadata.Role, role))
                    .Where(item => item.Metadata.TargetRegions.Contains(regionId))
                    .Select(item => GetTarget(item.Value))
                    .FirstOrDefault();
            }

            UpdateContent(contentControl, exportedItem);
        }

        private void UpdateContent(ContentControl contentControl, object targetItem)
        {
            Contract.Requires(contentControl != null);

            var currentItem = contentControl.Content;

            if (targetItem != currentItem)
            {
                ApplyContext(currentItem as IComposablePartWithContext, null);
                contentControl.Content = targetItem;
            }

            ApplyContext(targetItem as IComposablePartWithContext, CompositionContext);
        }

        private static void ApplyContext(IComposablePartWithContext item, object context)
        {
            if (item == null)
                return;

            item.CompositionContext = context;
        }
    }
}
