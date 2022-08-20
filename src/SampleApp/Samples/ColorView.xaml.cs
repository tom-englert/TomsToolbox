namespace SampleApp.Samples;

using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using TomsToolbox.Wpf.Composition.AttributedModel;

public class ColorItemViewModel
{
    public ColorItemViewModel(SolidColorBrush brush, string keyName)
    {
        Brush = brush;
        KeyName = keyName;
    }

    public SolidColorBrush Brush { get; }

    public string KeyName { get; }

    public double Luminance => ToGray(Brush.Color);

    public bool IsDark => Luminance < 128;

    public override string ToString()
    {
        return Brush.Color.ToString();
    }

    private static double ToGray(Color color)
    {
        return color.R * 0.3 + color.G * 0.59 + color.B * 0.11;
    }
}

[DataTemplate(typeof(ColorViewModel))]
public partial class ColorView
{
    public ColorView()
    {
        InitializeComponent();
        Items = _resourceKeys.Select(ToItemViewModel).ToArray();
    }

    public ColorItemViewModel[] Items
    {
        get => (ColorItemViewModel[])GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }
    public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
        "Items", typeof(ColorItemViewModel[]), typeof(ColorView), new PropertyMetadata(default(ColorItemViewModel[])));

    private ColorItemViewModel ToItemViewModel((ResourceKey resourceKey, string name) item)
    {
        return new ColorItemViewModel((SolidColorBrush)FindResource(item.resourceKey), item.name);
    }

    private void ColorView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        Clipboard.SetText(string.Join("\r\n", Items.Select(i =>
            $"<SolidColorBrush x:Key=\"{{x:Static SystemColors.{i.KeyName}}}\" Color=\"#{i.Brush.Color}\" />"
        )));
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property != ForegroundProperty)
            return;

        Items = _resourceKeys.Select(ToItemViewModel).ToArray();
    }


    private static readonly (ResourceKey Key, string name)[] _resourceKeys =
    {
        (SystemColors.ControlLightLightBrushKey,           nameof(SystemColors.ControlLightLightBrushKey)),
        (SystemColors.ControlLightBrushKey,                nameof(SystemColors.ControlLightBrushKey)),
        (SystemColors.ControlBrushKey,                     nameof(SystemColors.ControlBrushKey)),

        (SystemColors.ControlDarkBrushKey,                 nameof(SystemColors.ControlDarkBrushKey)),
        (SystemColors.ControlDarkDarkBrushKey,             nameof(SystemColors.ControlDarkDarkBrushKey)),
        (SystemColors.ControlTextBrushKey,                 nameof(SystemColors.ControlTextBrushKey)),

        (SystemColors.GrayTextBrushKey,                    nameof(SystemColors.GrayTextBrushKey)),

        (SystemColors.HighlightBrushKey,                   nameof(SystemColors.HighlightBrushKey)),
        (SystemColors.HighlightTextBrushKey,               nameof(SystemColors.HighlightTextBrushKey)),

        (SystemColors.InfoTextBrushKey,                    nameof(SystemColors.InfoTextBrushKey)),
        (SystemColors.InfoBrushKey,                        nameof(SystemColors.InfoBrushKey)),

        (SystemColors.MenuBrushKey,                        nameof(SystemColors.MenuBrushKey)),
        (SystemColors.MenuBarBrushKey,                     nameof(SystemColors.MenuBarBrushKey)),
        (SystemColors.MenuTextBrushKey,                    nameof(SystemColors.MenuTextBrushKey)),

        (SystemColors.WindowBrushKey,                      nameof(SystemColors.WindowBrushKey)),
        (SystemColors.WindowTextBrushKey,                  nameof(SystemColors.WindowTextBrushKey)),

        (SystemColors.ActiveCaptionBrushKey,               nameof(SystemColors.ActiveCaptionBrushKey)),
        (SystemColors.ActiveBorderBrushKey,                nameof(SystemColors.ActiveBorderBrushKey)),
        (SystemColors.ActiveCaptionTextBrushKey,           nameof(SystemColors.ActiveCaptionTextBrushKey)),

        (SystemColors.InactiveCaptionBrushKey,             nameof(SystemColors.InactiveCaptionBrushKey)),
        (SystemColors.InactiveBorderBrushKey,              nameof(SystemColors.InactiveBorderBrushKey)),
        (SystemColors.InactiveCaptionTextBrushKey,         nameof(SystemColors.InactiveCaptionTextBrushKey)),

        (TomsToolbox.Wpf.Styles.ResourceKeys.BorderBrush,   nameof(TomsToolbox.Wpf.Styles.ResourceKeys.BorderBrush)),
        (TomsToolbox.Wpf.Styles.ResourceKeys.DisabledBrush, nameof(TomsToolbox.Wpf.Styles.ResourceKeys.DisabledBrush))
    };

}