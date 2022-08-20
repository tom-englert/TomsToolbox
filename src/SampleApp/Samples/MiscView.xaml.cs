namespace SampleApp.Samples;

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

using TomsToolbox.Wpf.Composition.AttributedModel;

/// <summary>
/// Interaction logic for MiscView.xaml
/// </summary>
[DataTemplate(typeof(MiscViewModel))]
public partial class MiscView
{
    public MiscView()
    {
        InitializeComponent();
    }

    private void OperationStarted_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ((TextBox) sender).Text = DateTime.Now.ToString(CultureInfo.CurrentCulture);
    }
}