namespace TomsToolbox.Wpf.Composition.Mef2
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;

    internal class ExportAdapter<T> : IExport<T>
    {
        private readonly Lazy<T> _lazy;
        [CanBeNull]
        private readonly IDictionary<string, object> _metadata;

        public ExportAdapter([NotNull] Func<T> valueFactory, [CanBeNull] IDictionary<string, object> metadata)
        {
            _lazy = new Lazy<T>(valueFactory); 
            _metadata = metadata;
        }

        [CanBeNull]
        T IExport<T, IDictionary<string, object>>.Value => _lazy.Value;

        [CanBeNull]
        IDictionary<string, object> IExport<T, IDictionary<string, object>>.Metadata => _metadata;
    }
}
