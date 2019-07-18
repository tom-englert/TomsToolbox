namespace TomsToolbox.Composition
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Adapter for a delegate implementation of the <see cref="IExport{T,TMetadata}"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExportAdapter<T> : IExport<T>
    {
        [NotNull] 
        private readonly Func<T> _valueFactory;

        [CanBeNull]
        private readonly IMetadata _metadata;

        /// <summary>Initializes a new instance of the <see cref="ExportAdapter{T}"/> class.</summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportAdapter([NotNull] Func<T> valueFactory, [CanBeNull] IMetadata metadata)
        {
            _valueFactory = valueFactory;
            _metadata = metadata;
        }

        [CanBeNull]
        T IExport<T, IMetadata>.Value => _valueFactory();

        [CanBeNull]
        IMetadata IExport<T, IMetadata>.Metadata => _metadata;
    }
}
