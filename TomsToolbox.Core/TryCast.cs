namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Entry point to create the <see cref="TryCastWorker{TValue}"/>.
    /// </summary>
    public static class TryCastExtension
    {
        /// <summary>
        /// Creates the <see cref="TryCastWorker{TValue}"/> object to get a fluent notation for try-casting types.<para/>
        /// This pattern avoids heavily nested if (class is type) / else chains when testing for more than one possible cast.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="TryCastWorker{TValue}"/> object.</returns>
        /// <example>
        /// <code language="C#"><![CDATA[
        /// schedule.TryCast()
        ///     .When<SingleSchedule>(x => x.Remove())
        ///     .When<RecurrentSchedule>(x = > x.AddExceptionDate(date))
        ///     .ElseThrow();
        /// ]]></code>
        /// </example>
        public static TryCastWorker<TValue> TryCast<TValue>(this TValue value)
        {
            Contract.Ensures(Contract.Result<TryCastWorker<TValue>>() != null);

            return new TryCastWorker<TValue>(value);
        }
    }

    /// <summary>
    /// Provide fluent notation for try-casting types.<para/>
    /// Create this object using <see cref="TryCastExtension.TryCast{T}"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class TryCastWorker<TValue> : TryCastWorkerBase<TValue, object>
    {
        internal TryCastWorker(TValue value)
            : base(value)
        {
        }

        /// <summary>
        /// Tries to cast the value to <typeparamref name="TTarget"/>; if the cast succeeds, the action is executed.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>The object itself to continue with fluent notation.</returns>
        /// <remarks>
        /// If any previous method in the fluent chain has already succeeded, this method does nothing.
        /// </remarks>
        public TryCastWorker<TValue> When<TTarget>(Action<TTarget> action)
            where TTarget : TValue
        {
            Contract.Requires(action != null);
            Contract.Ensures(Contract.Result<TryCastWorker<TValue>>() != null);

            TryExecute<TTarget>(target => WrapAction(action, target));

            return this;
        }

        /// <summary>
        /// Executes the action if no previous cast has succeeded.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Else(Action<TValue> action)
        {
            Contract.Requires(action != null);

            TryExecute<TValue>(target => WrapAction(action, target));
        }

        /// <summary>
        /// Adds a return value to the fluent chain.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>The <see cref="TryCastWorker{TValue, TResult}"/> object.</returns>
        public TryCastWorker<TValue, TResult> Returning<TResult>()
        {
            Contract.Ensures(Contract.Result<TryCastWorker<TValue, TResult>>() != null);

            return new TryCastWorker<TValue, TResult>(Value);
        }

        /// <summary>
        /// Adds a return value to the fluent chain.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="defaultValue">The default value of the result if no cast succeeds.</param>
        /// <returns>
        /// The <see cref="TryCastWorker{TValue, TResult}" /> object.
        /// </returns>
        public TryCastWorker<TValue, TResult> Returning<TResult>(TResult defaultValue)
        {
            Contract.Ensures(Contract.Result<TryCastWorker<TValue, TResult>>() != null);

            return new TryCastWorker<TValue, TResult>(Value, defaultValue);
        }

        /// <summary>
        /// Wraps the action so it can be used where a function is expected.
        /// </summary>
        private static object WrapAction<TTarget>(Action<TTarget> action, TTarget target)
        {
            Contract.Requires(action != null);

            action(target);
            return null;
        }
    }

    /// <summary>
    /// Provide fluent notation for try-casting types and returning a result.<para/>
    /// Create this object using <see cref="TryCastWorker{T}.Returning{TResult}()"/>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class TryCastWorker<TValue, TResult>
        : TryCastWorkerBase<TValue, TResult>
    {
        internal TryCastWorker(TValue value)
            : base(value)
        {
        }

        internal TryCastWorker(TValue value, TResult defaultValue)
            : base(value, defaultValue)
        {
        }

        /// <summary>
        /// Gets the result of the action of the first succeeded cast.
        /// </summary>
        public TResult Result
        {
            get
            {
                return InternalResult;
            }
        }

        /// <summary>
        /// Tries to cast the value to <typeparamref name="TTarget"/>; if the cast succeeds, the action is executed and the result is stored in the <see cref="Result"/> property.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>The object itself to continue with fluent notation.</returns>
        /// <remarks>
        /// If any previous method in the fluent chain has already succeeded, this method does nothing.
        /// </remarks>
        public TryCastWorker<TValue, TResult> When<TTarget>(Func<TTarget, TResult> action)
            where TTarget : TValue
        {
            Contract.Requires(action != null);
            Contract.Ensures(Contract.Result<TryCastWorker<TValue, TResult>>() != null);

            TryExecute(action);

            return this;
        }

        /// <summary>
        /// Executes the action and returns the result if no previous cast has succeeded.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The result.</returns>
        public TResult Else(Func<TValue, TResult> action)
        {
            Contract.Requires(action != null);

            TryExecute(action);

            return InternalResult;
        }
    }

    /// <summary>
    /// Provide fluent notation for try-casting types and returning a result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class TryCastWorkerBase<TValue, TResult>
    {
        private readonly TValue _value;
        private bool _isResolved;

        internal TryCastWorkerBase(TValue value)
        {
            _value = value;
        }

        internal TryCastWorkerBase(TValue value, TResult defaultValue)
            : this(value)
        {
            InternalResult = defaultValue;
        }

        /// <summary>
        /// Gets the value to cast.
        /// </summary>
        protected TValue Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Gets the result of the action of the first succeeded cast.
        /// </summary>
        protected TResult InternalResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if none of the casts have succeeded.
        /// </summary>
        /// <returns>This method never returns, but throws the exception.</returns>
        /// <exception cref="System.InvalidOperationException">Encountered an unexpected type: 'type name'</exception>
        public TResult ElseThrow()
        {
            return ElseThrow("Encountered an unexpected type: " + ((_value == null) ? "(null)" : _value.GetType().FullName));
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException" /> if none of the casts have succeeded.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <returns>This method never returns, but throws the exception.</returns>
        /// <exception cref="System.InvalidOperationException"><paramref name="message"/></exception>
        public TResult ElseThrow(string message)
        {
            if (!_isResolved)
                throw new InvalidOperationException(message);

            return InternalResult;
        }

        /// <summary>
        /// Tries to cast the value and executes the action if the cast was successful.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="action">The action.</param>
        protected void TryExecute<TTarget>(Func<TTarget, TResult> action)
            where TTarget : TValue
        {
            Contract.Requires(action != null);

            if (_isResolved)
                return;

            if (!(_value is TTarget))
                return;

            var target = (TTarget)_value;

            InternalResult = action(target);

            _isResolved = true;
        }
    }
}
