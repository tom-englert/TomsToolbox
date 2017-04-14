using System.Diagnostics.Contracts;

namespace SampleApp.Samples
{
    using System.ComponentModel.Composition;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop.Composition;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    [DataTemplate(typeof(MapViewModel), Role = TemplateRoles.Content)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MapView
    {
        [ImportingConstructor]
        public MapView([NotNull] ICompositionHost compositionHost)
        {
            Contract.Requires(compositionHost != null);
            this.SetExportProvider(compositionHost.Container);

            InitializeComponent();
        }
    }
}
