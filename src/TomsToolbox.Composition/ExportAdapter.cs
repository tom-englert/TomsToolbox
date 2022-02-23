namespace TomsToolbox.Composition
{
    using System;

    /// <summary>
    /// Adapter for a delegate implementation of the <see cref="IExport{T,TMetadata}"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExportAdapter<T> : IExport<T>
        where T : class
    {
        private readonly Func<T> _valueFactory;

        private readonly IMetadata? _metadata;

        /// <summary>Initializes a new instance of the <see cref="ExportAdapter{T}"/> class.</summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportAdapter(Func<T> valueFactory, IMetadata? metadata)
        {
            _valueFactory = valueFactory;
            _metadata = metadata;
        }

        T IExport<T, IMetadata>.Value => _valueFactory();

        IMetadata? IExport<T, IMetadata>.Metadata => _metadata;
    }
}
