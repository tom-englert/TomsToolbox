namespace TomsToolbox.Wpf
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    public interface ILazy<T, TMetaData>
    {
        [CanBeNull]
        T Value { get; }
        
        [CanBeNull]
        TMetaData Metadata { get; }
    }

    public interface IExportProvider
    {
        event EventHandler<EventArgs> ExportsChanged;

        [NotNull, ItemNotNull]
        IEnumerable<T> GetExportedValues<T>();

        [NotNull, ItemNotNull]
        IEnumerable<ILazy<T, TMetaData>> GetExports<T, TMetaData>(string contractName);

        [NotNull, ItemNotNull]
        IEnumerable<ILazy<object, object>> GetExports([NotNull] Type type, [CanBeNull] Type metadataViewType, [CanBeNull] string contractName);

        /// <summary>Gets a metadata view object from a dictionary of loose metadata.</summary>
        /// <param name="item">The item containing the metadata as a collection of loose metadata.</param>
        /// <typeparam name="TMetadataView">The type of the metadata view object to get.</typeparam>
        /// <returns>A metadata view containing the specified metadata.</returns>
        [CanBeNull]
        TMetadataView GetMetadataView<TMetadataView>([CanBeNull] ILazy<object, object> item) where TMetadataView : class;
    }
}
