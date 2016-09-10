namespace TomsToolbox.Wpf.XamlExtensions
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    using TomsToolbox.Core;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// XAML helpers to ease visual composition usage.
    /// </summary>
    public static class VisualComposition
    {
        /// <summary>
        /// The error number shown in trace messages.
        /// </summary>
        public const int ErrorNumber = 9001;

        /// <summary>
        /// Occurs when the visual composition framework has detected an error.
        /// </summary>
        public static event EventHandler<TextEventArgs> Error;

        internal static void OnError(object sender, Exception ex)
        {
            Contract.Requires(ex != null);

            OnError(sender, ex.ToString());
        }

        internal static void OnError(object sender, string message)
        {
            Contract.Requires(message != null);

            PresentationTraceSources.DataBindingSource?.TraceEvent(TraceEventType.Error, ErrorNumber, message);

            Error?.Invoke(sender, new TextEventArgs(message));
        }

        /// <summary>
        /// Gets the region identifier.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <returns>The region identifier</returns>
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static string GetRegionId(Control obj)
        {
            Contract.Requires(obj != null);
            return (string)obj.GetValue(RegionIdProperty);
        }
        /// <summary>
        /// Sets the region identifier.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRegionId(Control obj, string value)
        {
            Contract.Requires(obj != null);
            obj.SetValue(RegionIdProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="P:TomsToolbox.Wpf.XamlExtensions.VisualComposition.RegionId"/> attached property
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// Attach this property to inject a visual composition behavior with this region id into the attached object.
        /// </summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty RegionIdProperty =
            DependencyProperty.RegisterAttached("RegionId", typeof(string), typeof(VisualComposition), new FrameworkPropertyMetadata(RegionId_Changed));

        private static void RegionId_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var id = e.NewValue as string;
            if (id == null)
                return;

            d.TryCast()
                .When<ItemsControl>(i => SetRegionId<ItemsControlCompositionBehavior>(d, id))
                .When<ContentControl>(c => SetRegionId<ContentControlCompositionBehavior>(d, id))
                .ElseThrow();
        }

        private static void SetRegionId<T>(DependencyObject d, string id)
            where T : Behavior, IVisualCompositionBehavior, new()
        {
            var behaviors = Interaction.GetBehaviors(d);
            Contract.Assume(behaviors != null);

            var behavior = behaviors.OfType<T>().FirstOrDefault();
            if (behavior == null)
            {
                behaviors.Add(new T { RegionId = id });
            }
            else
            {
                behavior.RegionId = id;
            }
        }
    }
}
