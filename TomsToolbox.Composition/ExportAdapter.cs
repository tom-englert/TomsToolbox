namespace TomsToolbox.Composition
{
    using System;
    using System.Collections.Generic;

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
        private readonly IDictionary<string, object> _metadata;

        /// <summary>Initializes a new instance of the <see cref="ExportAdapter{T}"/> class.</summary>
        /// <param name="valueFactory">The value factory.</param>
        /// <param name="metadata">The metadata.</param>
        public ExportAdapter([NotNull] Func<T> valueFactory, [CanBeNull] IDictionary<string, object> metadata)
        {
            _valueFactory = valueFactory;
            _metadata = metadata;
        }

        [CanBeNull]
        T IExport<T, IDictionary<string, object>>.Value => _valueFactory();

        [CanBeNull]
        IDictionary<string, object> IExport<T, IDictionary<string, object>>.Metadata => _metadata;
    }
}
