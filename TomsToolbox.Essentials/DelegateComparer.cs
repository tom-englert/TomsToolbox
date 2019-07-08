namespace TomsToolbox.Essentials
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <inheritdoc />
    /// <summary>
    /// <see cref="T:System.Collections.Generic.IComparer`1" /> implementation using a delegate function to compare the values.
    /// </summary>
    public class DelegateComparer<T> : IComparer<T>
    {
        [NotNull]
        private readonly Func<T, T, int> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public DelegateComparer([NotNull] Func<T, T, int> comparer)
        {
            _comparer = comparer;
        }

        /// <inheritdoc />
        public int Compare([CanBeNull] T x, [CanBeNull] T y)
        {
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                if (ReferenceEquals(x, null))
                    return ReferenceEquals(y, null) ? 0 : -1;

                if (ReferenceEquals(y, null))
                    return 1;
            }

            return _comparer(x, y);
        }
    }
}
