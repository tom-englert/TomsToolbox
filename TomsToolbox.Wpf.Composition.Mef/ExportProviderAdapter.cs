namespace TomsToolbox.Wpf.Composition.Mef
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;

    using JetBrains.Annotations;

    public class ExportProviderAdapter : IExportProvider
    {
        [NotNull]
        private readonly ExportProvider _exportProvider;

        private event EventHandler<EventArgs> _exportsChanged;

        public ExportProviderAdapter([NotNull] ExportProvider exportProvider)
        {
            _exportProvider = exportProvider;
            exportProvider.ExportsChanged += ExportProvider_ExportsChanged;
        }

        private void ExportProvider_ExportsChanged(object sender, ExportsChangeEventArgs e)
        {
            _exportsChanged?.Invoke(sender, e);
        }

        event EventHandler<EventArgs> IExportProvider.ExportsChanged
        {
            add => _exportsChanged += value;
            remove => _exportsChanged -= value;
        }

        IEnumerable<T> IExportProvider.GetExportedValues<T>()
        {
            return _exportProvider.GetExportedValues<T>();
        }

        IEnumerable<ILazy<T, TMetaData>> IExportProvider.GetExports<T, TMetaData>(string exportContractName)
        {
            return _exportProvider.GetExports<T, TMetaData>(exportContractName).Select(item => new LazyAdapter<T, TMetaData>(item));
        }

        IEnumerable<ILazy<object, object>> IExportProvider.GetExports(Type type, Type metadataViewType, string contractName)
        {
            return _exportProvider.GetExports(type, metadataViewType, contractName).Select(item => new LazyAdapter<object, object>(item));
        }

        [CanBeNull]
        TMetadataView IExportProvider.GetMetadataView<TMetadataView>([CanBeNull] ILazy<object, object> item)
        {
            return item?.Metadata is IDictionary<string, object> metadataDictionary ? AttributedModelServices.GetMetadataView<TMetadataView>(metadataDictionary) : null;
        }
    }
}
