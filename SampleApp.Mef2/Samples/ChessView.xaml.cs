namespace SampleApp.Mef2.Samples
{
    using TomsToolbox.Wpf.Composition.Mef2;

    /// <summary>
    /// Interaction logic for ChessView.xaml
    /// </summary>
    [DataTemplate(typeof(ChessViewModel))]
    public partial class ChessView
    {
        public ChessView()
        {
            InitializeComponent();
        }
    }
}
