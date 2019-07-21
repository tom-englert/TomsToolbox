namespace SampleApp.Samples
{
    using TomsToolbox.Wpf.Composition.AttributedModel;

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
