namespace TomsToolbox.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
#if NETSTANDARD1_0
    using System.Reflection;
#endif

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
            Contract.Requires(comparer != null);

            _comparer = comparer;
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
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

        [ContractInvariantMethod, UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_comparer != null);
        }
    }
}
