namespace TomsToolbox.Wpf;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using TomsToolbox.Essentials;

/// <summary>
/// Helper methods to parse or create text representations of a table.
/// </summary>
public static class TableHelper
{
    private const string Quote = "\"";

    /// <summary>
    /// The text column separator
    /// </summary>
    public const char TextColumnSeparator = '\t';

    /// <summary>
    /// Gets the effective CSV column separator for the current culture.
    /// </summary>
    public static char CsvColumnSeparator => CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," ? ';' : ',';

    /// <summary>
    /// Converts a table to a tab separated text string.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The string representation of the table.</returns>
    public static string ToTextString(this IList<IList<string>> table)
    {
        return ToString(table, TextColumnSeparator);
    }

    /// <summary>
    /// Converts a table to a comma separated text string.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The string representation of the table.</returns>
    /// <remarks>The separator is culture specific, i.e. if the NumberDecimalSeparator is a comma, a semicolon is used</remarks>
    public static string ToCsvString(this IList<IList<string>> table)
    {
        return ToString(table, CsvColumnSeparator);
    }

    /// <summary>
    /// Converts a table to a separated text string.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="separator">The column separator.</param>
    /// <returns>
    /// The string representation of the table.
    /// </returns>
    public static string ToString(this IList<IList<string>> table, char separator)
    {
        if ((table.Count == 1) && (table[0] != null) && (table[0].Count == 1) && string.IsNullOrWhiteSpace(table[0][0]))
            return Quote + (table[0][0] ?? string.Empty) + Quote;

        return string.Join(Environment.NewLine, table.Select(line => string.Join(separator.ToString(), line.Select(cell => Quoted(cell, separator)))));
    }

    /// <summary>
    /// Quotes the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>A quoted string if the string requires quoting; otherwise the original string.</returns>
    public static string Quoted(this string? value, char separator)
    {
        if (value.IsNullOrEmpty())
            return string.Empty;

        if (value.Any(IsLineFeed) || value.Contains(separator) || value.StartsWith(Quote, StringComparison.Ordinal))
        {
            return Quote + value.Replace(Quote, Quote + Quote) + Quote;
        }

        return value;
    }

    /// <summary>
    /// Parses the text representation of a table.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="separator">The column separator.</param>
    /// <returns>The table.</returns>
    public static IList<IList<string>>? ParseTable(this string text, char separator)
    {
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

        return table.Any(columns => columns?.Count != headerColumns?.Count) ? null : table;
    }

    private static IList<string> ReadTableLine(TextReader reader, char separator)
    {
        var columns = new List<string>();

        while (true)
        {
            columns.Add(ReadTableColumn(reader, separator));

            if ((char)reader.Peek() == separator)
            {
                reader.Read();
                continue;
            }

            if (reader.Peek() == '\r')
                reader.Read();
            if (reader.Peek() == '\n')
                reader.Read();

            break;
        }
        return columns;
    }

    private static string ReadTableColumn(TextReader reader, char separator)
    {
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
