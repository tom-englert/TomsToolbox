namespace TomsToolbox.Wpf.Composition.Mef
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

        public LazyAdapter([NotNull] Lazy<T> lazy, [CanBeNull] IDictionary<string, object> metadata)
        {
            _lazy = lazy;
            _metadata = metadata;
        }

        [CanBeNull]
        T ILazy<T, IDictionary<string, object>>.Value => _lazy.Value;

        [CanBeNull]
        IDictionary<string, object> ILazy<T, IDictionary<string, object>>.Metadata => _metadata;
    }
}
