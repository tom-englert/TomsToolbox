namespace TomsToolbox.Wpf
{
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// A static class providing the resource keys for the theme resources.
    /// </summary>
    public static class ResourceKeys
    {
        /// <summary>
        /// The key name for the <see cref="AutoToolTipTextBoxStyle"/>
        /// </summary>
        [NotNull] public static readonly string AutoToolTipTextBoxStyleKeyName = "AutoToolTipTextBoxStyle";
        /// <summary>
        /// A style for text boxes that automatically sets the tool tip if the text is trimmed.
        /// </summary>
        [NotNull] public static readonly ResourceKey AutoToolTipTextBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), AutoToolTipTextBoxStyleKeyName);

        /// <summary>
        /// The key name for the <see cref="SharedWidthContainerStyle"/>
        /// </summary>
        [NotNull] public static readonly string SharedWidthContainerStyleKeyName = "SharedWidthContainerStyle";
        /// <summary>
        /// Resource key for a style applying a shared with to associated container controls.
        /// </summary>
        [NotNull] public static readonly ResourceKey SharedWidthContainerStyle = new ComponentResourceKey(typeof(ResourceKeys), SharedWidthContainerStyleKeyName);

        /// <summary>
        /// The key name for the <see cref="ListBoxItemCheckBoxStyle"/>
        /// </summary>
        [NotNull] public static readonly string ListBoxItemCheckBoxStyleKeyName = "ListBoxItemCheckBoxStyle";
        /// <summary>
        /// List box/list view with check boxes: Style to be applied to the check box inside item or cell template. See e.g. http://msdn.microsoft.com/en-us/library/ms754143.aspx.
        /// </summary>
        [NotNull] public static readonly ResourceKey ListBoxItemCheckBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), ListBoxItemCheckBoxStyleKeyName);

        /// <summary>
        /// The key name for the <see cref="DataGridRowCheckBoxStyle"/>
        /// </summary>
        [NotNull] public static readonly string DataGridRowCheckBoxStyleKeyName = "DataGridRowCheckBoxStyle";
        /// <summary>
        /// Data grid with check boxes for row selection: Style to be applied to the check box inside the row header template.
        /// </summary>
        [NotNull] public static readonly ResourceKey DataGridRowCheckBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), DataGridRowCheckBoxStyleKeyName);

        /// <summary>
        /// The key name for the <see cref="DataGridCellStyle"/>
        /// </summary>
        [NotNull] public static readonly string DataGridCellStyleKeyName = "DataGridCellStyle";
        /// <summary>
        /// A copy of the original data grid cell style, extended with VerticalAlignment binding to control the vertical alignment of the content via the DataGrid.VerticalContentAlignment property. Also adds support for padding.
        /// </summary>
        [NotNull] public static readonly ResourceKey DataGridCellStyle = new ComponentResourceKey(typeof(ResourceKeys), DataGridCellStyleKeyName);

        /// <summary>
        /// The key name for the <see cref="ControlWithValidationErrorToolTipStyle"/>
        /// </summary>
        [NotNull] public static readonly string ControlWithValidationErrorToolTipStyleKeyName = "ControlWithValidationErrorToolTipStyle";
        /// <summary>
        /// A control that shows validation errors in the tool tip.
        /// </summary>
        [NotNull] public static readonly ResourceKey ControlWithValidationErrorToolTipStyle = new ComponentResourceKey(typeof(ResourceKeys), ControlWithValidationErrorToolTipStyleKeyName);

        /// <summary>
        /// The key name for the <see cref="CompositeMenuStyle"/>
        /// </summary>
        [NotNull] public static readonly string CompositeMenuStyleKeyName = "CompositeMenuStyle";
        /// <summary>
        /// A style to build composite menus.
        /// </summary>
        [NotNull] public static readonly ResourceKey CompositeMenuStyle = new ComponentResourceKey(typeof(ResourceKeys), CompositeMenuStyleKeyName);
    }
}
