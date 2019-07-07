namespace TomsToolbox.Wpf.Composition.Mef
{
    using System;

    using JetBrains.Annotations;

    public class LazyAdapter<T, TMetadata> : ILazy<T, TMetadata>
    {
        private readonly Lazy<T, TMetadata> _lazy;

        public LazyAdapter([NotNull] Lazy<T, TMetadata> lazy)
        {
            _lazy = lazy;
        }

        [CanBeNull]
        T ILazy<T, TMetadata>.Value => _lazy.Value;

        [CanBeNull]
        TMetadata ILazy<T, TMetadata>.Metadata => _lazy.Metadata;
    }
}
