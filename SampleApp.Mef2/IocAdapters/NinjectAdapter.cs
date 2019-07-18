namespace SampleApp.Mef2.IocAdapters
{
    using System.Windows.Data;

    using JetBrains.Annotations;

    using Ninject;
    using Ninject.Extensions.Conventions;

    using TomsToolbox.Composition;
    using TomsToolbox.Composition.Ninject;
    using TomsToolbox.Wpf.Converters;

    internal class NinjectAdapter : IIocAdapter
    {
        [CanBeNull]
        private IKernel _kernel;

        public IExportProvider Initialize()
        {
            var kernel = new StandardKernel();

            kernel.BindExports(GetType().Assembly);

            var exportProvider = new ExportProvider(kernel);

            kernel.Bind<IExportProvider>().ToConstant(exportProvider);
            kernel.Bind(syntax => syntax
                .From(typeof(CoordinatesToPointConverter).Assembly)
                .Select(type => typeof(IValueConverter).IsAssignableFrom(type))
                .BindToSelf());

            _kernel = kernel;
            return exportProvider;
        }

        public void Dispose()
        {
            _kernel?.Dispose();
        }
    }
}
