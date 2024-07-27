namespace SampleApp.Samples;

using System.Windows.Controls;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
///     Interaction logic for SmoothScrollingView.xaml
/// </summary>
[DataTemplate(typeof(SmoothScrollingViewModel))]
public partial class SmoothScrollingView : UserControl
{
    public SmoothScrollingView()
    {
        InitializeComponent();
    }
}
