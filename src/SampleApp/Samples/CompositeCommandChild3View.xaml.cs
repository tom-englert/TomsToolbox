namespace SampleApp.Samples;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for CommandViewChild2.xaml
/// </summary>
[DataTemplate(typeof(CompositeCommandChild3ViewModel))]
public partial class CompositeCommandChild3View
{
    public CompositeCommandChild3View()
    {
        InitializeComponent();
    }
}