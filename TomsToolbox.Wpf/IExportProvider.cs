namespace TomsToolbox.Wpf
{
    using System.Collections.Generic;

    public interface IExportProvider
    {
        IEnumerable<T> GetExportedValues<T>();
    }
}
