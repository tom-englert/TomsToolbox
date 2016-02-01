namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Helper methods to interchange data via clipboard.
    /// </summary>
    public static class ClipboardHelper
    {
        private const string Quote = "\"";
        private const char TextColumnSeparator = '\t';

        private static char CsvColumnSeparator
        {
            get
            {
                return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," ? ';' : ',';
            }
        }

        /// <summary>
        /// Gets the clipboard data as a table.
        /// </summary>
        /// <returns>The parsed clipboard data as a table, or <c>null</c> if the clipboard is empty or does not contain normalized table data.</returns>
        /// <remarks>CSV data is preferred over TEXT data, since e.g. Excel create ambiguous text format when cells in the last column contain line breaks.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static IList<IList<string>> GetClipboardDataAsTable()
        {
            Contract.Ensures((Contract.Result<IList<IList<string>>>() == null) || (Contract.Result<IList<IList<string>>>().Count > 0));

            var csv = Clipboard.GetData(DataFormats.CommaSeparatedValue) as string;
            if (!string.IsNullOrEmpty(csv))
                return ParseTable(csv, CsvColumnSeparator);

            var text = Clipboard.GetText();
            if (!string.IsNullOrEmpty(text))
                return ParseTable(text, TextColumnSeparator);

            return null;
        }

        /// <summary>
        /// Sets the clipboard data for the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <remarks>
        /// This method sets the TEXT (tab delimited) and CSV data. Like in Excel the CSV delimiter is either comma or semicolon, depending on the current culture. 
        /// </remarks>
        public static void SetClipboardData(this IList<IList<string>> table)
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

        internal static string ToTextString(this IList<IList<string>> table)
        {
            Contract.Requires(table != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return ToString(table, TextColumnSeparator);
        }

        internal static string ToCsvString(this IList<IList<string>> table)
        {
            Contract.Requires(table != null);
            Contract.Ensures(Contract.Result<string>() != null);

            return ToString(table, CsvColumnSeparator);
        }

        internal static string ToString(this IList<IList<string>> table, char separator)
        {
            Contract.Requires(table != null);
            Contract.Ensures(Contract.Result<string>() != null);

            if ((table.Count == 1) && (table[0].Count == 1) && string.IsNullOrWhiteSpace(table[0][0]))
                return Quote + (table[0][0] ?? string.Empty) + Quote;

            return string.Join(Environment.NewLine, table.Select(line => string.Join(separator.ToString(), line.Select(cell => Quoted(cell, separator)))));
        }

        internal static string Quoted(string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Any(IsLineFeed) || value.Contains(separator) || value.StartsWith(Quote, StringComparison.Ordinal))
            {
                return Quote + value.Replace(Quote, Quote + Quote) + Quote;
            }

            return value;
        }

        internal static IList<IList<string>> ParseTable(string text, char separator)
        {
            Contract.Requires(text != null);
            Contract.Ensures((Contract.Result<IList<IList<string>>>() == null) || Contract.Result<IList<IList<string>>>().Count > 0);

            var table = new List<IList<string>>();

            using (var reader = new StringReader(text))
            {
                while (reader.Peek() != -1)
                {
                    table.Add(ReadTableLine(reader, separator));
                }
            }

            if (!table.Any())
                return null;

            var headerColumns = table.First();

            return table.Any(columns => columns.Count != headerColumns.Count) ? null : table;
        }

        internal static IList<string> ReadTableLine(TextReader reader, char separator)
        {
            Contract.Requires(reader != null);
            Contract.Ensures(Contract.Result<IList<string>>() != null);
            Contract.Ensures(Contract.ForAll(Contract.Result<IList<string>>(), item => item != null));

            var columns = new List<string>();

            while (true)
            {
                columns.Add(ReadTableColumn(reader, separator));

                if ((char)reader.Peek() == separator)
                {
                    reader.Read();
                    continue;
                }

                while (IsLineFeed(reader.Peek()))
                {
                    reader.Read();
                }

                break;
            }

            Contract.Assume(Contract.ForAll(columns, item => item != null));
            return columns;
        }

        internal static string ReadTableColumn(TextReader reader, char separator)
        {
            Contract.Requires(reader != null);
            Contract.Ensures(Contract.Result<string>() != null);

            var stringBuilder = new StringBuilder();
            int nextChar;

            if (IsDoubleQuote(reader.Peek()))
            {
                reader.Read();

                while ((nextChar = reader.Read()) != -1)
                {
                    if (IsDoubleQuote(nextChar))
                    {
                        if (IsDoubleQuote(reader.Peek()))
                        {
                            reader.Read();
                            stringBuilder.Append((char)nextChar);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        stringBuilder.Append((char)nextChar);
                    }
                }
            }
            else
            {
                while ((nextChar = reader.Peek()) != -1)
                {
                    if (IsLineFeed(nextChar) || (nextChar == separator))
                        break;

                    reader.Read();
                    stringBuilder.Append((char)nextChar);
                }
            }

            return stringBuilder.ToString();
        }

        private static bool IsDoubleQuote(int c)
        {
            return (c == '"');
        }

        private static bool IsLineFeed(int c)
        {
            return (c == '\r') || (c == '\n');
        }

        private static bool IsLineFeed(char c)
        {
            return IsLineFeed((int)c);
        }
    }
}
