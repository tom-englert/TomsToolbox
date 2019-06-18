namespace TomsToolbox.Core
{
    using System;

    using JetBrains.Annotations;
#if !PORTABLE && !NETSTANDARD1_0
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// A typed version of the <see cref="WeakReference"/>
    /// </summary>
    /// <typeparam name="T">The type of the object that is tracked.</typeparam>
#if !PORTABLE && !NETSTANDARD1_0
    [Serializable]
#endif
    public sealed class WeakReference<T> : WeakReference where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class.
        /// </summary>
        /// <param name="target">An object to track.</param>
        public WeakReference([CanBeNull] T target)
            : base(target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}" /> class.
        /// </summary>
        /// <param name="target"> An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. If true, the object is tracked after finalization; if false, the object is only tracked until finalization.</param>
        public WeakReference([CanBeNull] T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
        }

#if !PORTABLE && !NETSTANDARD1_0
        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReference{T}"/> class.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize the current <see cref="T:System.WeakReference" /> object.</param>
        /// <param name="context">(Reserved) Describes the source and destination of the serialized stream specified by <paramref name="info" />.</param>
        // ReSharper disable once AnnotateNotNullParameter
        private WeakReference([NotNull] SerializationInfo info, StreamingContext context)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Gets or sets the object (the target) referenced by the current <see cref="T:System.WeakReference" /> object.
        /// </summary>
        /// <returns>null if the object referenced by the current <see cref="T:System.WeakReference" /> object has been garbage collected; otherwise, a reference to the object referenced by the current <see cref="T:System.WeakReference" /> object.</returns>
        [CanBeNull]
        public new T Target => (T)base.Target;

        /// <summary>
        /// Tries to the get the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>True if target is valid.</returns>
        [ContractAnnotation("target:notnull => true")]
        public bool TryGetTarget([CanBeNull] out T target)
        {
            target = Target;
            return (target != null);
        }
    }
}
