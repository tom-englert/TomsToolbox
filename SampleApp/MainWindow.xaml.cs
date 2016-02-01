namespace SampleApp
{
    using System.ComponentModel.Composition;
    using TomsToolbox.Desktop.Composition;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow
    {
        [ImportingConstructor]
        public MainWindow(ICompositionHost compositionHost)
        {
            this.SetExportProvider(compositionHost.Container);

            InitializeComponent();
        }
    }
}