namespace TomsToolbox.Core
{
    using System;

    /// <summary>
    /// Interface implemented by objects that support a generic changed event.
    /// </summary>
    public interface INotifyChanged
    {
        /// <summary>
        /// Occurs when the object has changed.
        /// </summary>
        event EventHandler Changed;
    }
}