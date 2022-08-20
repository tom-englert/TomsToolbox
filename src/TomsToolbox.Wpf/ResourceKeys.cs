namespace TomsToolbox.Wpf;

using System.Windows;

/// <summary>
/// A static class providing the resource keys for the theme resources.
/// </summary>
public static class ResourceKeys
{
    /// <summary>
    /// A style for text boxes that automatically sets the tool tip if the text is trimmed.
    /// </summary>
    public static readonly ResourceKey AutoToolTipTextBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "AutoToolTipTextBoxStyle");

    /// <summary>
    /// Resource key for a style applying a shared with to associated container controls.
    /// </summary>
    public static readonly ResourceKey SharedWidthContainerStyle = new ComponentResourceKey(typeof(ResourceKeys), "SharedWidthContainerStyle");

    /// <summary>
    /// List box/list view with check boxes: Style to be applied to the check box inside item or cell template. See e.g. http://msdn.microsoft.com/en-us/library/ms754143.aspx.
    /// </summary>
    public static readonly ResourceKey ListBoxItemCheckBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListBoxItemCheckBoxStyle");

    /// <summary>
    /// Data grid with check boxes for row selection: Style to be applied to the check box inside the row header template.
    /// </summary>
    public static readonly ResourceKey DataGridRowCheckBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridRowCheckBoxStyle");

    /// <summary>
    /// A copy of the original data grid cell style, extended with VerticalAlignment binding to control the vertical alignment of the content via the DataGrid.VerticalContentAlignment property. Also adds support for padding.
    /// </summary>
    public static readonly ResourceKey DataGridCellStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCellStyle");

    /// <summary>
    /// A control that shows validation errors in the tool tip.
    /// </summary>
    public static readonly ResourceKey ControlWithValidationErrorToolTipStyle = new ComponentResourceKey(typeof(ResourceKeys), "ControlWithValidationErrorToolTipStyle");
}