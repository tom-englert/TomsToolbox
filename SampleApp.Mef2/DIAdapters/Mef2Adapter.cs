namespace SampleApp.Mef2.DIAdapters
{
    using System;
    using System.Composition;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Windows.Data;

    using JetBrains.Annotations;

    using TomsToolbox.Composition;
    using TomsToolbox.Composition.Mef2;
    using TomsToolbox.Wpf.Converters;

    internal sealed class DIAdapter : IDisposable
    {
        [CanBeNull]
        private CompositionHost? _container;
        [CanBeNull]
        private static IExportProvider? _exportProvider;

        [Export(typeof(IExportProvider))]
        [CanBeNull]
        public IExportProvider? ExportProvider => _exportProvider;
        
        public IExportProvider Initialize()
        {
            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IValueConverter>().Export();

            var configuration = new ContainerConfiguration()
                .WithAssembly(typeof(MainWindow).Assembly, conventions)
                .WithAssembly(GetType().Assembly)
                .WithAssembly(typeof(CoordinatesToPointConverter).Assembly, conventions);

            var container = configuration.CreateContainer();

            var exportProvider = new ExportProviderAdapter(container);

            _exportProvider = exportProvider;
            _container = container;

            return exportProvider;
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}
