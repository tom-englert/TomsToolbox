namespace TomsToolbox.Essentials
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for the <see cref="CultureInfo"/> class.
    /// </summary>
    public static class CultureInfoExtensions
    {
        [NotNull] private static readonly Dictionary<CultureInfo, CultureInfo[]> _childCache = new Dictionary<CultureInfo, CultureInfo[]>();

        /// <summary>
        /// Returns an enumeration of the ancestor elements of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        [ItemNotNull]
        [NotNull]
        public static IEnumerable<CultureInfo> GetAncestors([NotNull] this CultureInfo self)
        {
            var item = self.Parent;

            while (!string.IsNullOrEmpty(item.Name))
            {
                yield return item;
                item = item.Parent;
            }
        }

        /// <summary>
        /// Returns an enumeration of elements that contain this element, and the ancestors of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        [ItemNotNull]
        [NotNull]
        public static IEnumerable<CultureInfo> GetAncestorsAndSelf([NotNull] this CultureInfo self)
        {
            var item = self;

            while (!string.IsNullOrEmpty(item.Name))
            {
                yield return item;
                item = item.Parent;
            }
        }

        /// <summary>
        /// Enumerates the immediate children of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The immediate children of the specified item.</returns>
        [NotNull, ItemNotNull]
        public static ICollection<CultureInfo> GetChildren([NotNull] this CultureInfo item)
        {
            return _childCache.ForceValue(item, CreateChildList)!;
        }

        [NotNull, ItemNotNull]
        private static CultureInfo[] CreateChildList([CanBeNull] CultureInfo? parent)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Where(child => child?.Parent.Equals(parent) == true).ToArray();
        }

        /// <summary>
        /// Enumerates all descendants of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The descendants of the item.</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<CultureInfo> GetDescendants([NotNull] this CultureInfo item)
        {
            foreach (var child in item.GetChildren())
            {
                yield return child;

                foreach (var d in child.GetDescendants())
                {
                    yield return d;
                }
            }
        }
    }
}
