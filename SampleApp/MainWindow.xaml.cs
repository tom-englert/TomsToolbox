namespace SampleApp
{
    using System.ComponentModel.Composition;
    using System.Diagnostics.Contracts;
    using System.Windows;

    using JetBrains.Annotations;

    using TomsToolbox.Desktop.Composition;
    using TomsToolbox.Wpf.Composition;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class MainWindow
    {
        [ImportingConstructor]
        public MainWindow([NotNull] ICompositionHost compositionHost)
        {
            Contract.Requires(compositionHost != null);
            this.SetExportProvider(compositionHost.Container);

            InitializeComponent();
        }

        private void ShowPopup_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow { Owner = this }.ShowDialog();
        }
    }
}