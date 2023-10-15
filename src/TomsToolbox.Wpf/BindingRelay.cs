namespace TomsToolbox.Wpf;

using System.Windows;

/// <summary>
/// A simple helper to relay the data context to objects that don't live in the visual tree.
/// </summary>
/// <example>
/// <code language="xaml"><![CDATA[
/// <DataGrid>
///   <DataGrid.Resources>
///     <local:BindingRelay x:Key="relay" DataContext="{Binding}"/>
///   </DataGrid.Resources>
///   <DataGrid.Columns>
///     <DataGridTextColumn Visibility="{Binding DataContext.MyColumnVisibility, Source={StaticResource relay}}"/>
///   </DataGrid.Columns>
/// </DataGrid>]]></code>
/// </example>
public class BindingRelay : Freezable
{
    /// <summary>
    /// When implemented in a derived class, creates a new instance of the <see cref="T:System.Windows.Freezable" /> derived class.
    /// </summary>
    /// <returns>
    /// The new instance.
    /// </returns>
    protected override Freezable CreateInstanceCore()
    {
        return new BindingRelay();
    }

    /// <summary>
    /// Gets or sets the data to be relayed, usually the DataContext of the owning object.
    /// </summary>
    public object DataContext
    {
        get => GetValue(DataContextProperty);
        set => SetValue(DataContextProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="DataContext"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DataContextProperty = DependencyProperty.Register("DataContext", typeof(object), typeof(BindingRelay));
}
