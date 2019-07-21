namespace TomsToolbox.Core
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// This module is deprecated. Code has been moved to TomsToolbox.Essentials to avoid naming confusions with .Net Core.
    /// </summary>
    [Obsolete("This module is deprecated. Code has been moved to TomsToolbox.Essentials", true)]
    public class Deprecated
    {
        /// <summary>
        /// Dummy method.
        /// </summary>
        /// <returns>null</returns>
        [CanBeNull]
        public object Dummy()
        {
            return null;
        }
    }
}
