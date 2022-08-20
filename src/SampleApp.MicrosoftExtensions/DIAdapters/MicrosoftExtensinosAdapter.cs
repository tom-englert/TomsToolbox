namespace SampleApp.MicrosoftExtensions.DIAdapters;

using System.Linq;
using System.Windows.Data;

using Microsoft.Extensions.DependencyInjection;

using TomsToolbox.Composition;
using TomsToolbox.Composition.MicrosoftExtensions;
using TomsToolbox.Wpf.Converters;

internal static class DIAdapter
{
    public static IExportProvider Initialize()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.BindExports(typeof(MainWindow).Assembly);

        foreach (var valueConverterType in typeof(CoordinatesToPointConverter).Assembly.GetTypes().Where(type => typeof(IValueConverter).IsAssignableFrom(type) && !type.IsAbstract))
        {
            serviceCollection.AddSingleton(valueConverterType);
        }

        return new ExportProviderAdapter(serviceCollection);
    }
}