namespace TomsToolbox.Core
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Interface implemented by objects that support a generic changed event.
    /// </summary>
    [ContractClass(typeof (NotifyChangedContract))]
    public interface INotifyChanged
    {
        /// <summary>
        /// Occurs when the object has changed.
        /// </summary>
        event EventHandler Changed;
    }

    [ContractClassFor(typeof (INotifyChanged))]
    abstract class NotifyChangedContract : INotifyChanged
    {
        event EventHandler INotifyChanged.Changed
        {
            add
            {
                Contract.Requires(value != null);
                throw new NotImplementedException();
            }
            remove
            {
                Contract.Requires(value != null);
                throw new NotImplementedException();
            }
        }
    }
}