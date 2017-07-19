namespace TomsToolbox.Wpf.Styles
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using JetBrains.Annotations;

    /// <summary>
    /// Resource keys for the styles.
    /// </summary>
    public static class ResourceKeys
    {
        #region Brushes

        /// <summary>
        /// The disabled brush
        /// </summary>
        [NotNull] public static readonly ResourceKey DisabledBrush = new ComponentResourceKey(typeof(ResourceKeys), "DisabledBrush");

        /// <summary>
        /// The border brush
        /// </summary>
        [NotNull] public static readonly ResourceKey BorderBrush = new ComponentResourceKey(typeof(ResourceKeys), "BorderBrush");

        /// <summary>
        /// The dark accent brush
        /// </summary>
        [NotNull] public static readonly ResourceKey AccentDarkBrush = new ComponentResourceKey(typeof(ResourceKeys), "AccentDarkBrush");

        #endregion Brushes

        #region Styles

        /// <summary>
        /// The resource key for the <see cref="Button"/> style.
        /// </summary>
        [DefaultStyle(typeof(Button))]
        [NotNull] public static readonly ResourceKey ButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "ButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="CheckBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(CheckBox))]
        [NotNull] public static readonly ResourceKey CheckBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "CheckBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="CheckBox"/> style of a check box inside a data grid.
        /// </summary>
        [NotNull] public static readonly ResourceKey DataGridCheckBoxEditingElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCheckBoxEditingElementStyle");

        /// <summary>
        /// The resource key for the <see cref="CheckBox"/> style of a check box inside a data grid.
        /// </summary>
        [NotNull] public static readonly ResourceKey DataGridCheckBoxElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCheckBoxElementStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(ComboBox))]
        [NotNull] public static readonly ResourceKey ComboBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ComboBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style of a combo box inside a data grid.
        /// </summary>
        [NotNull] public static readonly ResourceKey DataGridComboBoxEditingElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridComboBoxEditingElementStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style of a combo box inside a data grid.
        /// </summary>
        [NotNull] public static readonly ResourceKey DataGridComboBoxElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridComboBoxElementStyle");

        /// <summary>
        /// The resource key for the <see cref="ContextMenu"/> style.
        /// </summary>
        [DefaultStyle(typeof(ContextMenu))]
        [NotNull] public static readonly ResourceKey ContextMenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "ContextMenuStyle");

        /// <summary>
        /// The resource key for a composite <see cref="ContextMenu"/> style.
        /// </summary>
        [NotNull] public static readonly ResourceKey CompositeContextMenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "CompositeContextMenuStyle");

        /// <summary>
        /// The resource key for the <see cref="Menu"/> style.
        /// </summary>
        [DefaultStyle(typeof(Menu))]
        [NotNull] public static readonly ResourceKey MenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "MenuStyle");

        /// <summary>
        /// The resource key for the <see cref="MenuItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(MenuItem))]
        [NotNull] public static readonly ResourceKey MenuItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "MenuItemStyle");

        /// <summary>
        /// The resource key for the <see cref="Separator"/> style.
        /// </summary>
        [NotNull] public static readonly ResourceKey MenuItemSeparatorStyle = new ComponentResourceKey(typeof(ResourceKeys), "MenuItemSeparatorStyle");

        /// <summary>
        /// The resource key for the <see cref="GridSplitter"/> style.
        /// </summary>
        [DefaultStyle(typeof(GridSplitter))]
        [NotNull] public static readonly ResourceKey GridSplitterStyle = new ComponentResourceKey(typeof(ResourceKeys), "GridSplitterStyle");

        /// <summary>
        /// The resource key for the <see cref="GroupBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(GroupBox))]
        [NotNull] public static readonly ResourceKey GroupBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "GroupBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ListBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(ListBox))]
        [NotNull] public static readonly ResourceKey ListBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ListBoxItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(ListBoxItem))]
        [NotNull] public static readonly ResourceKey ListBoxItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListBoxItemStyle");

        /// <summary>
        /// The resource key for the <see cref="RadioButton"/> style.
        /// </summary>
        [DefaultStyle(typeof(RadioButton))]
        [NotNull] public static readonly ResourceKey RadioButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "RadioButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="TabControl"/> style.
        /// </summary>
        [DefaultStyle(typeof(TabControl))]
        [NotNull] public static readonly ResourceKey TabControlStyle = new ComponentResourceKey(typeof(ResourceKeys), "TabControlStyle");

        /// <summary>
        /// The resource key for the <see cref="TabItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(TabItem))]
        [NotNull] public static readonly ResourceKey TabItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "TabItemStyle");

        /// <summary>
        /// The resource key for the <see cref="TextBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(TextBox))]
        [NotNull] public static readonly ResourceKey TextBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "TextBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ToggleButton"/> style.
        /// </summary>
        [DefaultStyle(typeof(ToggleButton))]
        [NotNull] public static readonly ResourceKey ToggleButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToggleButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="ToolTip"/> style.
        /// </summary>
        [DefaultStyle(typeof(ToolTip))]
        [NotNull] public static readonly ResourceKey ToolTipStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolTipStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGrid"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGrid))]
        [NotNull] public static readonly ResourceKey DataGridStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridCell"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridCell))]
        [NotNull] public static readonly ResourceKey DataGridCellStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCellStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridRow"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridRow))]
        [NotNull] public static readonly ResourceKey DataGridRowStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridRowStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridColumnHeader"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridColumnHeader))]
        [NotNull] public static readonly ResourceKey DataGridColumnHeaderStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridColumnHeaderStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridRowHeader"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridRowHeader))]
        [NotNull] public static readonly ResourceKey DataGridRowHeaderStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridRowHeaderStyle");

        /// <summary>
        /// The resource key for the <see cref="Window"/> style.
        /// </summary>
        [DefaultStyle(typeof(Window))]
        [NotNull] public static readonly ResourceKey WindowStyle = new ComponentResourceKey(typeof(ResourceKeys), "WindowStyle");

        /// <summary>
        /// The resource key for the icon control style.
        /// </summary>
        /// <remarks>
        /// Add a style with this key to your application resources to override the applications icon in the window.
        /// This style will be applied to a control in the top left corner of the window caption to display the applications icon.
        /// Using a control styles enables to use any WPF element to design the icon, not only bitmaps.
        /// </remarks>
        [NotNull] public static readonly ResourceKey IconControlStyle = new ComponentResourceKey(typeof(ResourceKeys), "IconControlStyle");

        #endregion // Styles
    }
}
