namespace TomsToolbox.Composition
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// An adapter to provide a dictionary with metadata as <see cref="IMetadata"/>
    /// </summary>
    public class MetadataAdapter : IMetadata
    {
        private readonly IDictionary<string, object> _metadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataAdapter"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MetadataAdapter(IDictionary<string, object> metadata)
        {
            _metadata = metadata;
        }

        /// <inheritdoc />
        public object GetValue(string name)
        {
            return _metadata[name];
        }

        /// <inheritdoc />
        public bool TryGetValue(string name, [NotNullWhen(true)] out object? value)
        {
            return _metadata.TryGetValue(name, out value);
        }
    }
}