namespace SampleApp;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for WindowTitleDecoratorView.xaml
/// </summary>
[DataTemplate(typeof(WindowTitleDecoratorViewModel))]
public partial class WindowTitleDecoratorView
{
    public WindowTitleDecoratorView()
    {
        InitializeComponent();
    }
}