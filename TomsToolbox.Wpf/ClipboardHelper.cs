namespace TomsToolbox.Wpf
{
    using System.Collections.Generic;
    using System.Windows;

    using JetBrains.Annotations;

    /// <summary>
    /// Helper methods to interchange data via clipboard.
    /// </summary>
    public static class ClipboardHelper
    {
        /// <summary>
        /// Gets the clipboard data as a table.
        /// </summary>
        /// <returns>The parsed clipboard data as a table, or <c>null</c> if the clipboard is empty or does not contain normalized table data.</returns>
        /// <remarks>If no TEXT is present in the clipboard, CSV data is used.</remarks>
        [CanBeNull, ItemNotNull]
        public static IList<IList<string>> GetClipboardDataAsTable()
        {
            var text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
                return TableHelper.ParseTable(text, TableHelper.TextColumnSeparator);

            var csv = Clipboard.GetData(DataFormats.CommaSeparatedValue) as string;
            if (!string.IsNullOrEmpty(csv))
                return TableHelper.ParseTable(csv, TableHelper.CsvColumnSeparator);

            return null;
        }

        /// <summary>
        /// Sets the clipboard data for the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <remarks>
        /// This method sets the TEXT (tab delimited) and CSV data. Like in Excel the CSV delimiter is either comma or semicolon, depending on the current culture.
        /// </remarks>
        public static void SetClipboardData([CanBeNull, ItemNotNull] this IList<IList<string>> table)
        {
            if (table == null)
            {
                Clipboard.Clear();
                return;
            }

            var textString = table.ToTextString();
            var csvString = table.ToCsvString();

            var dataObject = new DataObject();

            dataObject.SetText(textString);
            dataObject.SetText(csvString, TextDataFormat.CommaSeparatedValue);

            Clipboard.SetDataObject(dataObject);
        }
    }
}
