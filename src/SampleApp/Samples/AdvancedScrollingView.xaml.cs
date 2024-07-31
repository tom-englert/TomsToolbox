namespace SampleApp.Samples;

using System.Windows.Controls;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
///     Interaction logic for SmoothScrollingView.xaml
/// </summary>
[DataTemplate(typeof(AdvancedScrollingViewModel))]
public partial class AdvancedScrollingView : UserControl
{
    public AdvancedScrollingView()
    {
        InitializeComponent();
    }
}
