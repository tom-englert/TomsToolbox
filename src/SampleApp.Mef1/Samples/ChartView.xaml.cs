namespace SampleApp.Mef1.Samples;

using TomsToolbox.Wpf.Composition.Mef;

/// <summary>
/// Interaction logic for CharView.xaml
/// </summary>
[DataTemplate(typeof(ChartViewModel))]
public partial class ChartView
{
    public ChartView()
    {
        InitializeComponent();
    }
}