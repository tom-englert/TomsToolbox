namespace TomsToolbox.Essentials
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Extensions for <see cref="WeakReference{T}"/>
    /// </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        /// Gets the target of the weak reference, if the target is still alive; otherwise null.
        /// </summary>
        /// <typeparam name="T">The type of the object referenced.</typeparam>
        /// <param name="weakReference">The weak reference.</param>
        /// <returns>The target of the weak reference, if the target is still alive; otherwise null.</returns>
        [CanBeNull]
        public static T GetTargetOrDefault<T>([CanBeNull] this WeakReference<T> weakReference)
        where T : class
        {
            if (weakReference == null)
                return null;

            return weakReference.TryGetTarget(out var target) ? target : default;
        }
    }
}
