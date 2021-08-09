namespace TomsToolbox.Essentials
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="CultureInfo"/> class.
    /// </summary>
    public static class CultureInfoExtensions
    {
        private static readonly Dictionary<CultureInfo, CultureInfo[]> _childCache = new();

        /// <summary>
        /// Returns an enumeration of the ancestor elements of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        public static IEnumerable<CultureInfo> GetAncestors(this CultureInfo self)
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
        public static IEnumerable<CultureInfo> GetAncestorsAndSelf(this CultureInfo self)
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
        public static ICollection<CultureInfo> GetChildren(this CultureInfo item)
        {
            return _childCache.ForceValue(item, CreateChildList);
        }

        private static CultureInfo[] CreateChildList(CultureInfo? parent)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures).Where(child => child?.Parent.Equals(parent) == true).ToArray();
        }

        /// <summary>
        /// Enumerates all descendants of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The descendants of the item.</returns>
        public static IEnumerable<CultureInfo> GetDescendants(this CultureInfo item)
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
