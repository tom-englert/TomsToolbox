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
        [NotNull]
        public static readonly ResourceKey ButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "ButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="CheckBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(CheckBox))]
        [NotNull]
        public static readonly ResourceKey CheckBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "CheckBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="CheckBox"/> style of a check box inside a data grid.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey DataGridCheckBoxEditingElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCheckBoxEditingElementStyle");

        /// <summary>
        /// The resource key for the <see cref="CheckBox"/> style of a check box inside a data grid.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey DataGridCheckBoxElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCheckBoxElementStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(ComboBox))]
        [NotNull]
        public static readonly ResourceKey ComboBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ComboBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style of a combo box inside a data grid.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey DataGridComboBoxEditingElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridComboBoxEditingElementStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style of a combo box inside a data grid.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey DataGridComboBoxElementStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridComboBoxElementStyle");

        /// <summary>
        /// The resource key for the <see cref="Expander"/> style.
        /// </summary>
        [DefaultStyle(typeof(Expander))]
        [NotNull]
        public static readonly ResourceKey ExpanderStyle = new ComponentResourceKey(typeof(ResourceKeys), "ExpanderStyle");

        /// <summary>
        /// The resource key for the <see cref="ContextMenu"/> style.
        /// </summary>
        [DefaultStyle(typeof(ContextMenu))]
        [NotNull]
        public static readonly ResourceKey ContextMenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "ContextMenuStyle");

        /// <summary>
        /// The resource key for the <see cref="Menu"/> style.
        /// </summary>
        [DefaultStyle(typeof(Menu))]
        [NotNull]
        public static readonly ResourceKey MenuStyle = new ComponentResourceKey(typeof(ResourceKeys), "MenuStyle");

        /// <summary>
        /// The resource key for the <see cref="MenuItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(MenuItem))]
        [NotNull]
        public static readonly ResourceKey MenuItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "MenuItemStyle");

        /// <summary>
        /// The resource key for the <see cref="Separator"/> style.
        /// </summary>
        [DefaultStyle(typeof(Separator), typeof(MenuItem), nameof(MenuItem.SeparatorStyleKey))]
        [NotNull]
        public static readonly ResourceKey MenuItemSeparatorStyle = new ComponentResourceKey(typeof(ResourceKeys), "MenuItemSeparatorStyle");

        /// <summary>
        /// The resource key for the <see cref="GridSplitter"/> style.
        /// </summary>
        [DefaultStyle(typeof(GridSplitter))]
        [NotNull]
        public static readonly ResourceKey GridSplitterStyle = new ComponentResourceKey(typeof(ResourceKeys), "GridSplitterStyle");

        /// <summary>
        /// The resource key for the <see cref="GroupBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(GroupBox))]
        [NotNull]
        public static readonly ResourceKey GroupBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "GroupBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ListBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(ListBox))]
        [NotNull]
        public static readonly ResourceKey ListBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ListBoxItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(ListBoxItem))]
        [NotNull]
        public static readonly ResourceKey ListBoxItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListBoxItemStyle");

        /// <summary>
        /// The resource key for the <see cref="ListView"/> style.
        /// </summary>
        [DefaultStyle(typeof(ListView))]
        [NotNull]
        public static readonly ResourceKey ListViewStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListViewStyle");
        /// <summary>

        /// The resource key for the <see cref="ListViewItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(ListViewItem))]
        [NotNull]
        public static readonly ResourceKey ListViewItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "ListViewItemStyle");

        /// <summary>
        /// The resource key for the <see cref="ScrollViewer"/> style of the grid view.
        /// </summary>
        [NotNull]
        public static readonly ResourceKey GridViewScrollViewerStyle = new ComponentResourceKey(typeof(ResourceKeys), "GridViewScrollViewerStyle");

        /// <summary>
        /// The resource key for the <see cref="GridViewColumnHeader"/> style.
        /// </summary>
        [DefaultStyle(typeof(GridViewColumnHeader))]
        [NotNull]
        public static readonly ResourceKey GridViewColumnHeaderStyle = new ComponentResourceKey(typeof(ResourceKeys), "GridViewColumnHeaderStyle");

        /// The resource key for the <see cref="RadioButton"/> style.
        /// </summary>
        [DefaultStyle(typeof(RadioButton))]
        [NotNull]
        public static readonly ResourceKey RadioButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "RadioButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="ScrollBar"/> style.
        /// </summary>
        [DefaultStyle(typeof(ScrollBar))]
        [NotNull]
        public static readonly ResourceKey ScrollBarStyle = new ComponentResourceKey(typeof(ResourceKeys), "ScrollBarStyle");

        /// <summary>
        /// The resource key for the <see cref="ScrollViewer"/> style.
        /// </summary>
        [DefaultStyle(typeof(ScrollViewer))]
        [NotNull]
        public static readonly ResourceKey ScrollViewerStyle = new ComponentResourceKey(typeof(ResourceKeys), "ScrollViewerStyle");

        /// <summary>
        /// The resource key for the <see cref="TabControl"/> style.
        /// </summary>
        [DefaultStyle(typeof(TabControl))]
        [NotNull]
        public static readonly ResourceKey TabControlStyle = new ComponentResourceKey(typeof(ResourceKeys), "TabControlStyle");

        /// <summary>
        /// The resource key for the <see cref="TabItem"/> style.
        /// </summary>
        [DefaultStyle(typeof(TabItem))]
        [NotNull]
        public static readonly ResourceKey TabItemStyle = new ComponentResourceKey(typeof(ResourceKeys), "TabItemStyle");

        /// <summary>
        /// The resource key for the <see cref="TextBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(TextBox))]
        [NotNull]
        public static readonly ResourceKey TextBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "TextBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="PasswordBox"/> style.
        /// </summary>
        [DefaultStyle(typeof(PasswordBox))]
        [NotNull]
        public static readonly ResourceKey PasswordBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "PasswordBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ToggleButton"/> style.
        /// </summary>
        [DefaultStyle(typeof(ToggleButton))]
        [NotNull]
        public static readonly ResourceKey ToggleButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToggleButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="ToolTip"/> style.
        /// </summary>
        [DefaultStyle(typeof(ToolTip))]
        [NotNull]
        public static readonly ResourceKey ToolTipStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolTipStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGrid"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGrid))]
        [NotNull]
        public static readonly ResourceKey DataGridStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridCell"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridCell))]
        [NotNull]
        public static readonly ResourceKey DataGridCellStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridCellStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridRow"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridRow))]
        [NotNull]
        public static readonly ResourceKey DataGridRowStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridRowStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridColumnHeader"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridColumnHeader))]
        [NotNull]
        public static readonly ResourceKey DataGridColumnHeaderStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridColumnHeaderStyle");

        /// <summary>
        /// The resource key for the <see cref="DataGridRowHeader"/> style.
        /// </summary>
        [DefaultStyle(typeof(DataGridRowHeader))]
        [NotNull]
        public static readonly ResourceKey DataGridRowHeaderStyle = new ComponentResourceKey(typeof(ResourceKeys), "DataGridRowHeaderStyle");

        /// <summary>
        /// The resource key for the <see cref="Window"/> style.
        /// </summary>
        [DefaultStyle(typeof(Window))]
        [NotNull]
        public static readonly ResourceKey WindowStyle = new ComponentResourceKey(typeof(ResourceKeys), "WindowStyle");

        /// <summary>
        /// The resource key for the <see cref="ToolBar"/> style.
        /// </summary>
        [DefaultStyle(typeof(ToolBar))]
        [NotNull]
        public static readonly ResourceKey ToolBarStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolBarStyle");
        /// <summary>
        /// The resource key for the <see cref="Button"/> style inside a tool bar.
        /// </summary>
        [DefaultStyle(typeof(Button), typeof(ToolBar), nameof(ToolBar.ButtonStyleKey))]
        [NotNull]
        public static readonly ResourceKey ToolBarButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolBarButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="ToggleButton"/> style inside a tool bar.
        /// </summary>
        [DefaultStyle(typeof(ToggleButton), typeof(ToolBar), nameof(ToolBar.ToggleButtonStyleKey))]
        [NotNull]
        public static readonly ResourceKey ToolBarToggleButtonStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolBarToggleButtonStyle");

        /// <summary>
        /// The resource key for the <see cref="TextBox"/> style inside a tool bar.
        /// </summary>
        [DefaultStyle(typeof(TextBox), typeof(ToolBar), nameof(ToolBar.TextBoxStyleKey))]
        [NotNull]
        public static readonly ResourceKey ToolBarTextBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolBarTextBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="ComboBox"/> style inside a tool bar.
        /// </summary>
        [DefaultStyle(typeof(ComboBox), typeof(ToolBar), nameof(ToolBar.ComboBoxStyleKey))]
        [NotNull]
        public static readonly ResourceKey ToolBarComboBoxStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolBarComboBoxStyle");

        /// <summary>
        /// The resource key for the <see cref="Separator"/> style inside a tool bar.
        /// </summary>
        [DefaultStyle(typeof(Separator), typeof(ToolBar), nameof(ToolBar.SeparatorStyleKey))]
        [NotNull]
        public static readonly ResourceKey ToolBarSeparatorStyle = new ComponentResourceKey(typeof(ResourceKeys), "ToolBarSeparatorStyle");

        /// <summary>
        /// The resource key for the icon control style.
        /// </summary>
        /// <remarks>
        /// Add a style with this key to your application resources to override the applications icon in the window.
        /// This style will be applied to a control in the top left corner of the window caption to display the applications icon.
        /// Using a control styles enables to use any WPF element to design the icon, not only bitmaps.
        /// </remarks>
        [NotNull]
        public static readonly ResourceKey IconControlStyle = new ComponentResourceKey(typeof(ResourceKeys), "IconControlStyle");

        #endregion // Styles
    }
}
