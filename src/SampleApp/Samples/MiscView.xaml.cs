namespace SampleApp.Samples;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

    private bool _useFilter;

    private void FilteredItems_OnFilter(object sender, FilterEventArgs e)
    {
        e.Accepted = !_useFilter || e.Item is KeyValuePair<int, string> item && item.Key % 2 == 0;
    }

    private void UseFilter_Changed(object sender, RoutedEventArgs e)
    {
        _useFilter = (sender as CheckBox)?.IsChecked ?? false;

        (FindResource("FilteredItems") as CollectionViewSource)?.View?.Refresh();
    }
}
