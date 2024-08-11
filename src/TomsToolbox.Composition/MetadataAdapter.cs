namespace TomsToolbox.Composition;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if NET6_0_OR_GREATER

using System.Globalization;
using System.Reflection;

using TomsToolbox.Essentials;

#endif

/// <summary>
/// An adapter to provide a dictionary with metadata as <see cref="IMetadata"/>
/// </summary>
public class MetadataAdapter : IMetadata
{
    private readonly IDictionary<string, object?> _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataAdapter"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public MetadataAdapter(IDictionary<string, object?> metadata)
    {
        _metadata = metadata;
    }

    /// <inheritdoc />
    public object? GetValue(string name)
    {
        return _metadata[name];
    }

    /// <inheritdoc />
    public bool TryGetValue(string name, [NotNullWhen(true)] out object? value)
    {
        return _metadata.TryGetValue(name, out value) && value != null;
    }

    /// <summary>
    /// Create a typed metadata view for the specified metadata.
    /// </summary>
    /// <param name="metadata">The untyped metadata</param>
    /// <returns>The typed metadata</returns>
    public static TMetadataView Create<TMetadataView>(IMetadata? metadata)
    {
#if NET6_0_OR_GREATER
        return BuildMetadata<TMetadataView>(metadata);
#else
        throw new NotSupportedException("This method is not supported in this target framework.");
#endif
    }

#if NET6_0_OR_GREATER
    private static TMetadataView BuildMetadata<TMetadataView>(IMetadata? inner)
    {
        var metadataView = DispatchProxy.Create<TMetadataView, MetadataProxy>();

        (metadataView as MetadataProxy)!.Metadata = inner;

        return metadataView;
    }

    private class MetadataProxy : DispatchProxy
    {
        public IMetadata? Metadata { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is null)
                return null;

            if (Metadata is null)
                return DefaultValue.CreateDefault(targetMethod.ReturnType);

            try
            {
                var methodName = targetMethod.Name;
                if (!methodName.StartsWith("get_", StringComparison.Ordinal))
                    return DefaultValue.CreateDefault(targetMethod.ReturnType);

                var propertyName = methodName[4..];

                if (Metadata.TryGetValue(propertyName, out var value) != true)
                    return DefaultValue.CreateDefault(targetMethod.ReturnType);

                if (value.GetType() == targetMethod.ReturnType)
                    return value;

                return Convert.ChangeType(value, targetMethod.ReturnType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return DefaultValue.CreateDefault(targetMethod.ReturnType);
            }
        }
    }
#endif
}

