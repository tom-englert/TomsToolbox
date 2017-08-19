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

    /// <summary>
    /// <see cref="IEqualityComparer{T}"/> implementation using a delegate function to compare the values.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class DelegateEqualityComparer<T> : IEqualityComparer<T>
    {
        [NotNull]
        private readonly Func<T, T, bool> _comparer;
        [NotNull]
        private readonly Func<T, int> _hashCodeGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateEqualityComparer{T}"/> class,
        /// using <see cref="object.Equals(object, object)"/> and <see cref="object.GetHashCode()"/>
        /// </summary>
        public DelegateEqualityComparer()
            : this((a, b) => object.Equals(a, b), x => ReferenceEquals(x, null) ? 0 : x.GetHashCode())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="selector">The selector that selects the object to compare, if e.g. two objects can be compared by a single property.</param>
        public DelegateEqualityComparer([NotNull] Func<T, object> selector)
        {
            Contract.Requires(selector != null);

            _comparer = (a, b) => Equals(selector(a), selector(b));
            _hashCodeGenerator = obj => selector(obj)?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateEqualityComparer{T}" /> class.
        /// </summary>
        /// <param name="comparer">The compare function.</param>
        /// <param name="hashCodeGenerator">The hash code generator.</param>
        public DelegateEqualityComparer([NotNull] Func<T, T, bool> comparer, [NotNull] Func<T, int> hashCodeGenerator)
        {
            Contract.Requires(comparer != null);
            Contract.Requires(hashCodeGenerator != null);

            _comparer = comparer;
            _hashCodeGenerator = hashCodeGenerator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateEqualityComparer{T}" /> class.
        /// </summary>
        /// <param name="selector">The selector that selects the object to compare, if e.g. two objects can be compared by a single property.</param>
        /// <param name="comparer">The compare function.</param>
        /// <param name="hashCodeGenerator">The hash code generator.</param>
        public DelegateEqualityComparer([NotNull] Func<T, object> selector, [NotNull] Func<object, object, bool> comparer, [NotNull] Func<object, int> hashCodeGenerator)
        {
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);
            Contract.Requires(hashCodeGenerator != null);

            _comparer = (a, b) => comparer(selector(a), selector(b));
            _hashCodeGenerator = obj => hashCodeGenerator(selector(obj));
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
        public bool Equals([CanBeNull] T x, [CanBeNull] T y)
        {
            // ReSharper disable once PossibleNullReferenceException
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                if (ReferenceEquals(x, null))
                    return ReferenceEquals(y, null);

                if (ReferenceEquals(y, null))
                    return false;
            }

            return _comparer(x, y);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
        public int GetHashCode([CanBeNull] T obj)
        {
            // ReSharper disable All
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                if (ReferenceEquals(obj, null))
                    return 0;
            }

            return _hashCodeGenerator(obj);
            // ReSharper enable All
        }

        [ContractInvariantMethod, UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_comparer != null);
            Contract.Invariant(_hashCodeGenerator != null);
        }
    }
}
