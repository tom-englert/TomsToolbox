namespace TomsToolbox.Composition;

using System;

/// <summary>
/// Adapter for a delegate implementation of the <see cref="IExport{T,TMetadata}"/> interface.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TMetadataView"></typeparam>
public class ExportAdapter<T, TMetadataView> : IExport<T, TMetadataView>
    where T : class
    where TMetadataView : class
{
    private readonly Func<T?> _valueFactory;

    private readonly TMetadataView? _metadata;

    /// <summary>Initializes a new instance of the <see cref="ExportAdapter{T,TMetadataView}"/> class.</summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="metadata">The metadata.</param>
    public ExportAdapter(Func<T?> valueFactory, TMetadataView? metadata)
    {
        _valueFactory = valueFactory;
        _metadata = metadata;
    }

    T? IExport<T, TMetadataView>.Value => _valueFactory();

    TMetadataView? IExport<T, TMetadataView>.Metadata => _metadata;
}


/// <summary>
/// Adapter for a delegate implementation of the <see cref="IExport{T}"/> interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExportAdapter<T> : ExportAdapter<T, IMetadata>, IExport<T>
    where T : class
{
    /// <summary>Initializes a new instance of the <see cref="ExportAdapter{T}"/> class.</summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <param name="metadata">The metadata.</param>
    public ExportAdapter(Func<T?> valueFactory, IMetadata? metadata) : base(valueFactory, metadata)
    {
    }
}
