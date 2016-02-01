namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="Maybe{T}"/> monad implementation.
    /// <see href="http://www.codeproject.com/Articles/109026/Chained-null-checks-and-the-Maybe-monad"/>
    /// <see href="http://smellegantcode.wordpress.com/2008/12/11/the-maybe-monad-in-c/"/>
    /// </summary>
    public static class MaybeExtensions
    {
        /// <summary>
        /// Generates the Maybe monad for the specified value.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="selector">The selector to get the target from the value.</param>
        /// <returns>The <see cref="Maybe{T}"/> with the target as value.</returns>
        public static Maybe<TTarget> Maybe<TSource, TTarget>(this TSource value, Func<TSource, TTarget> selector)
            where TSource:class
            where TTarget:class
        {
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<Maybe<TTarget>>() != null);

            return new Maybe<TSource>(value).Select(selector);
        }

        /// <summary>
        /// Generates the Maybe monad for the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Maybe{T}"/> with the value.</returns>
        public static Maybe<TValue> Maybe<TValue>(this TValue value)
            where TValue : class
        {
            Contract.Ensures(Contract.Result<Maybe<TValue>>() != null);

            return new Maybe<TValue>(value);
        }
    }

    /// <summary>
    /// A Maybe monad implementation.
    /// <see href="http://www.codeproject.com/Articles/109026/Chained-null-checks-and-the-Maybe-monad"/>
    /// <see href="http://smellegantcode.wordpress.com/2008/12/11/the-maybe-monad-in-c/"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Maybe<T> where T:class
    {
        private readonly T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Maybe{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Maybe(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns a new <see cref="Maybe{T}"/> for the target.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns>A new <see cref="Maybe{T}"/> for the target</returns>
        public Maybe<TTarget> Select<TTarget>(Func<T, TTarget> selector)
            where TTarget : class
        {
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<Maybe<TTarget>>() != null);

            return new Maybe<TTarget>((_value != null) ? selector(_value) : null);
        }

        /// <summary>
        /// Returns the inner value if not null, else returns default(T).
        /// </summary>
        /// <returns>
        /// The inner value if not null, else default(T).
        /// </returns>
        public T Return()
        {
            return Return(default(T));
        }

        /// <summary>
        /// Returns the if the inner value if not null, else returns the default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The inner value if not null, else default(T).
        /// </returns>
        public T Return(T defaultValue)
        {
            return _value ?? defaultValue;
        }

        /// <summary>
        /// Returns the value extracted from the specified selector if the inner value is not null, else returns default(TTarget).
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns>
        /// The value extracted from the specified selector if the inner value is not null, else default(TTarget).
        /// </returns>
        public TTarget Return<TTarget>(Func<T, TTarget> selector)
        {
            Contract.Requires(selector != null);

            return Return(selector, default(TTarget));
        }

        /// <summary>
        /// Returns the value extracted from the specified selector if the inner value is not null, else returns the default value.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value extracted from the specified selector if the inner value is not null, else default(TTarget).
        /// </returns>
        public TTarget Return<TTarget>(Func<T, TTarget> selector, TTarget defaultValue)
        {
            Contract.Requires(selector != null);

            return (_value != null) ? selector(_value) : defaultValue;
        }

        /// <summary>
        /// Executes the specified action if the inner value is not null.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>this</returns>
        public Maybe<T> Do(Action<T> action)
        {
            Contract.Requires(action != null);
            Contract.Ensures(Contract.Result<Maybe<T>>() != null);

            if (_value != null)
                action(_value);

            return this;
        }

        /// <summary>
        /// Checks the condition and returns an empty Maybe if the condition fails.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>this or an empty maybe.</returns>
        public Maybe<T> If(Func<T, bool> condition)
        {
            Contract.Requires(condition != null);
            Contract.Ensures(Contract.Result<Maybe<T>>() != null);

            if ((_value != null) && !condition(_value))
            {
                return new Maybe<T>(null);
            }

            return this;
        }

        /// <summary>
        /// Checks the condition and returns an empty Maybe if the condition succeeds.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>this or an empty maybe.</returns>
        public Maybe<T> Unless(Func<T, bool> condition)
        {
            Contract.Requires(condition != null);
            Contract.Ensures(Contract.Result<Maybe<T>>() != null);

            if ((_value != null) && condition(_value))
            {
                return new Maybe<T>(null);
            }

            return this;
        }
    }
}
