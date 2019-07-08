namespace TomsToolbox.Wpf.Composition.Mef2
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using TomsToolbox.Essentials;

    internal class LazyAdapter<T> : ILazy<T>
    {
        private readonly Lazy<T> _lazy;
        [CanBeNull]
        private readonly IDictionary<string, object> _metadata;

        public LazyAdapter([NotNull] Func<T> valueFactory, [CanBeNull] IDictionary<string, object> metadata)
        {
            _lazy = new Lazy<T>(valueFactory); 
            _metadata = metadata;
        }

        [CanBeNull]
        T ILazy<T, IDictionary<string, object>>.Value => _lazy.Value;

        [CanBeNull]
        IDictionary<string, object> ILazy<T, IDictionary<string, object>>.Metadata => _metadata;
    }
}
