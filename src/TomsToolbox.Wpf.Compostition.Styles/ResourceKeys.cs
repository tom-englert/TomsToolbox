namespace TomsToolbox.Wpf.Composition.Styles
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

        /// <summary>
        /// A style to build composite menus.
        /// </summary>
        [NotNull] public static readonly ResourceKey CompositeMenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "CompositeMenuStyle");

        /// <summary>
        /// The resource key for the <see cref="Window"/> style.
        /// </summary>
        [NotNull] public static readonly ResourceKey WindowStyle = new ComponentResourceKey(typeof(ResourceKeys), "WindowStyle");
    }
}
