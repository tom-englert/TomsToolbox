namespace TomsToolbox.Wpf.XamlExtensions
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    using JetBrains.Annotations;

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

        internal static void OnError([CanBeNull] object sender, [NotNull] Exception ex)
        {

            OnError(sender, ex.ToString());
        }

        internal static void OnError([CanBeNull] object sender, [NotNull] string message)
        {

            PresentationTraceSources.DataBindingSource?.TraceEvent(TraceEventType.Error, ErrorNumber, message);

            Error?.Invoke(sender, new TextEventArgs(message));
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
        [NotNull] public static readonly DependencyProperty RegionIdProperty =
            DependencyProperty.RegisterAttached("RegionId", typeof(string), typeof(VisualComposition), new FrameworkPropertyMetadata(RegionId_Changed));

        private static void RegionId_Changed([CanBeNull] DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is string id))
                return;

            d.TryCast()
                .When<ItemsControl>(i => SetRegionIdInternal<ItemsControlCompositionBehavior>(d, id))
                .When<ContentControl>(c => SetRegionIdInternal<ContentControlCompositionBehavior>(d, id))
                .ElseThrow();
        }

        private static void SetRegionIdInternal<T>([CanBeNull] DependencyObject d, [CanBeNull] string id)
            where T : Behavior, IVisualCompositionBehavior, new()
        {
            var behaviors = Interaction.GetBehaviors(d);

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
