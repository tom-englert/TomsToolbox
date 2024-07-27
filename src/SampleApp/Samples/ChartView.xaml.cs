namespace SampleApp.Samples;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for CharView.xaml
/// </summary>
[DataTemplate(typeof(ChartViewModel))]
// [NonShared]
public partial class ChartView
{
    public ChartView()
    {
        InitializeComponent();
    }
}
