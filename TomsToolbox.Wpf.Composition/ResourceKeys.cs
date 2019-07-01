namespace TomsToolbox.Wpf.Composition
{
    using System.Windows;
    using System.Windows.Controls;

    using JetBrains.Annotations;

    /// <summary>
    /// Resource keys for the styles.
    /// </summary>
    public static class ResourceKeys
    {
        /// <summary>
        /// The resource key for a composite <see cref="ContextMenu"/> style.
        /// </summary>
        [NotNull] public static readonly ResourceKey CompositeContextMenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "CompositeContextMenuStyle");
    }
}
