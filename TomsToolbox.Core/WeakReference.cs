namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// A typed version of the <see cref="WeakReference"/>
    /// </summary>
    /// <typeparam name="T">The type of the object that is tracked.</typeparam>
#if !PORTABLE
    [Serializable]
#endif
    public sealed class WeakReference<T> : WeakReference where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class.
        /// </summary>
        /// <param name="target">An object to track.</param>
        public WeakReference(T target)
            : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}" /> class.
        /// </summary>
        /// <param name="target"> An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If true, the object is tracked after finalization; if false, the object is only tracked until finalization.</param>
        public WeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

#if !PORTABLE
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize the current <see cref="T:System.WeakReference" /> object.</param>
        /// <param name="context">(Reserved) Describes the source and destination of the serialized stream specified by <paramref name="info" />.</param>
        private WeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference" /> object.
        /// </summary>
        /// <returns>null if the object referenced by the current <see cref="T:System.WeakReference" /> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference" /> object.</returns>
        public new T Target
        {
            get
            {
                return (T)base.Target;
            }
        }

        /// <summary>
        /// Tries to the get the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>True if target is valid.</returns>
        public bool TryGetTarget(out T target)
        {
            Contract.Ensures((Contract.Result<bool>() == false) || (Contract.ValueAtReturn(out target) != null));

            target = Target;
            return (target != null);
        }
    }
}
