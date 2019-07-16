namespace TomsToolbox.Wpf.Composition.Mef
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

        public ExportAdapter([NotNull] Lazy<T> lazy, [CanBeNull] IDictionary<string, object> metadata)
        {
            _lazy = lazy;
            _metadata = metadata;
        }

        [CanBeNull]
        T IExport<T, IDictionary<string, object>>.Value => _lazy.Value;

        [CanBeNull]
        IDictionary<string, object> IExport<T, IDictionary<string, object>>.Metadata => _metadata;
    }
}
