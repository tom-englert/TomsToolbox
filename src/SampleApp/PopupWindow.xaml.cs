namespace SampleApp;

using System.Windows.Input;

/// <summary>
/// Interaction logic for PopupWindow.xaml
/// </summary>
public partial class PopupWindow
{
    public PopupWindow()
    {
        InitializeComponent();
    }

    private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Close();
    }
}