namespace TomsToolbox.Desktop
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Extension methods for the <see cref="CultureInfo"/> class.
    /// </summary>
    public static class CultureInfoExtensions
    {
        private static readonly Dictionary<CultureInfo, CultureInfo[]> ChildCache = new Dictionary<CultureInfo, CultureInfo[]>();

        /// <summary>
        /// Returns an enumeration of the ancestor elements of this element.
        /// </summary>
        /// <param name="self">The starting element.</param>
        /// <returns>The ancestor list.</returns>
        [NotNull]
        public static IEnumerable<CultureInfo> GetAncestors([NotNull] this CultureInfo self)
        {
            Contract.Requires(self != null);
            Contract.Ensures(Contract.Result<IEnumerable<CultureInfo>>() != null);

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
        [NotNull]
        public static IEnumerable<CultureInfo> GetAncestorsAndSelf([NotNull] this CultureInfo self)
        {
            Contract.Requires(self != null);
            Contract.Ensures(Contract.Result<IEnumerable<CultureInfo>>() != null);

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
        [NotNull]
        public static ICollection<CultureInfo> GetChildren([NotNull] this CultureInfo item)
        {
            Contract.Requires(item != null);
            Contract.Ensures(Contract.Result<ICollection<CultureInfo>>() != null);

            var children = ChildCache.ForceValue(item, CreateChildList);
            Contract.Assume(children != null); // because CreateChildList always returns != null
            return children;
        }

        [NotNull]
        private static CultureInfo[] CreateChildList(CultureInfo parent)
        {
            Contract.Ensures(Contract.Result<CultureInfo[]>() != null);

            return CultureInfo.GetCultures(CultureTypes.AllCultures).Where(child => child.Parent.Equals(parent)).ToArray();
        }

        /// <summary>
        /// Enumerates all descendants of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The descendants of the item.</returns>
        [NotNull]
        public static IEnumerable<CultureInfo> GetDescendants([NotNull] this CultureInfo item)
        {
            Contract.Requires(item != null);
            Contract.Ensures(Contract.Result<IEnumerable<CultureInfo>>() != null);

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
