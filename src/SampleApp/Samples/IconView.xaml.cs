namespace SampleApp.Samples;

using System.ComponentModel;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for IconView.xaml
/// </summary>
[DataTemplate(typeof(IconViewModel))]
public partial class IconView
{
    public IconView()
    {
        InitializeComponent();
    }
}

[VisualCompositionExport(RegionId.Main, Sequence = 5)]
public partial class IconViewModel : INotifyPropertyChanged
{
    public override string ToString()
    {
        return "Icon";
    }
}
