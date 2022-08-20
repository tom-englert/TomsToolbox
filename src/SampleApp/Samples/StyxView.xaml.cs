namespace SampleApp.Samples;

using System.Windows;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for Styx.xaml
/// </summary>
[DataTemplate(typeof(StyxViewModel))]
public partial class Styx
{
    public Styx()
    {
        InitializeComponent();
    }

    private void LargeFont_Checked(object? sender, RoutedEventArgs e)
    {
        FontSize = 24;
    }

    private void LargeFont_Unchecked(object? sender, RoutedEventArgs e)
    {
        SetValue(FontSizeProperty, DependencyProperty.UnsetValue);
    }
}