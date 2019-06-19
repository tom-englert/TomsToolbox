namespace TomsToolbox.Core
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

#if NETSTANDARD1_0
    using System.Reflection;
#endif

    /// <inheritdoc />
    /// <summary>
    /// <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation using a delegate function to compare the values.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public class DelegateEqualityComparer<T> : IEqualityComparer<T>
    {
        [NotNull]
        private readonly Func<T, T, bool> _comparer;
        [NotNull]
        private readonly Func<T, int> _hashCodeGenerator;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TomsToolbox.Core.DelegateEqualityComparer`1" /> class,
        /// using <see cref="M:System.Object.Equals(System.Object,System.Object)" /> and <see cref="M:System.Object.GetHashCode" />
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
            _comparer = (a, b) => comparer(selector(a), selector(b));
            _hashCodeGenerator = obj => hashCodeGenerator(selector(obj));
        }

        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                if (ReferenceEquals(x, null))
                    return ReferenceEquals(y, null);

                if (ReferenceEquals(y, null))
                    return false;
            }

            return _comparer(x, y);
        }

        /// <inheritdoc />
        public int GetHashCode([CanBeNull] T obj)
        {
            if (!typeof(T).GetTypeInfo().IsValueType)
            {
                if (ReferenceEquals(obj, null))
                    return 0;
            }

            return _hashCodeGenerator(obj);
        }
    }
}
