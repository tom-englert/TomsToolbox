namespace TomsToolbox.Wpf.Composition.XamlExtensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    using Microsoft.Xaml.Behaviors;

    using TomsToolbox.Essentials;

    /// <summary>
    /// XAML helpers to ease visual composition usage.
    /// </summary>
    public static class VisualComposition
    {
        /// <summary>
        /// The contract name for visual composition exports.
        /// </summary>
        public const string ExportContractName = "VisualComposition-86E8D1EF-1322-46B4-905C-115AAD63533D";

        /// <summary>
        /// The error number shown in trace messages.
        /// </summary>
        public const int ErrorNumber = 9001;

        /// <summary>
        /// Occurs when the visual composition framework has detected an error.
        /// </summary>
        public static event EventHandler<TextEventArgs> Error;
        /// <summary>
        /// Occurs when the visual composition framework logs some action.
        /// </summary>
        public static event EventHandler<TextEventArgs> Trace;

        internal static void OnError([CanBeNull] object sender, [NotNull] Exception ex)
        {
            OnError(sender, ex.ToString());
        }

        internal static void OnError([CanBeNull] object sender, [NotNull] string message)
        {
            PresentationTraceSources.DataBindingSource?.TraceEvent(TraceEventType.Error, ErrorNumber, message);

            Error?.Invoke(sender, new TextEventArgs(message));
        }

        internal static void OnTrace([CanBeNull] object sender, [NotNull] string message)
        {
            Trace?.Invoke(sender, new TextEventArgs(message));
        }

        /// <summary>
        /// Gets the region identifier.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <returns>The region identifier</returns>
        [CanBeNull]
        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        [AttachedPropertyBrowsableForType(typeof(ItemsControl))]
        public static string GetRegionId([NotNull] Control obj)
        {
            return (string)obj.GetValue(RegionIdProperty);
        }
        /// <summary>
        /// Sets the region identifier.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public static void SetRegionId([NotNull] Control obj, [CanBeNull] string value)
        {
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
        [NotNull]
        public static readonly DependencyProperty RegionIdProperty =
            DependencyProperty.RegisterAttached("RegionId", typeof(string), typeof(VisualComposition), new FrameworkPropertyMetadata(RegionId_Changed));

        private static void RegionId_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is string id))
                return;

            switch (d)
            {
                case ItemsControl i:
                    SetRegionIdInternal<ItemsControlCompositionBehavior>(i, id);
                    break;
                case ContentControl c:
                    SetRegionIdInternal<ContentControlCompositionBehavior>(c, id);
                    break;
                default: 
                    throw new InvalidOperationException("unsupported type: " + d?.GetType());
            }
        }

        private static void SetRegionIdInternal<T>([NotNull] DependencyObject d, [CanBeNull] string id)
            where T : Behavior, IVisualCompositionBehavior, new()
        {
            try
            {
                var behaviors = Interaction.GetBehaviors(d);

                var behavior = behaviors.OfType<T>().FirstOrDefault();
                if (behavior == null)
                {
                    behaviors.Add(new T { RegionId = id });
                    OnTrace(d, $"SetRegion: Attached the behavior {typeof(T)} to the target {d.GetType()}");
                }
                else
                {
                    behavior.RegionId = id;
                    OnTrace(d, $"SetRegion: Updated the region id of behavior {typeof(T)} on the target {d.GetType()}");
                }
            }
            catch (Exception ex)
            {
                OnError(d, $"SetRegion: Failed to attach the behavior {typeof(T)} to the target {d.GetType()}: " + ex);
            }
        }
    }
}
