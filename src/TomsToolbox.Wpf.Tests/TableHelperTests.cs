#nullable disable
namespace TomsToolbox.Wpf.Tests;

using System.Collections.Generic;
using System.Linq;

using Xunit;

public class TableHelperTests
{
    [Fact]
    public void TableHelper_ParseQuotedStrings()
    {
        const string sourceText = "\"\"\"$(SolutionDir).nuget\\nuget.exe\"\" pack \"\"$(ProjectPath)\"\" -OutputDirectory \"\"$(SolutionDir)..\\Bin\\Deploy\\.\"\" -IncludeReferencedProjects -Prop Configuration=$(ConfigurationName)\"";
        const string targetText = "\"$(SolutionDir).nuget\\nuget.exe\" pack \"$(ProjectPath)\" -OutputDirectory \"$(SolutionDir)..\\Bin\\Deploy\\.\" -IncludeReferencedProjects -Prop Configuration=$(ConfigurationName)";
        var table = sourceText.ParseTable('\t');

        Assert.NotNull(table);
        Assert.Single(table);
        Assert.Single(table[0]);
        Assert.Single(table[0]);
        Assert.Equal(targetText, table[0][0]);
    }

    [Fact]
    public void TableHelper_QuotedStringsWithLineBreaks_RoundTrip()
    {
        IList<string> line1 = new[] { "L1\r\nC1", "L1C2\r\n" };
        IList<string> line2 = new[] { "L2C1", "\r\nL2C2" };
        var sourceTable = new[] { line1, line2 };
        var expected = "\"L1\r\nC1\";\"L1C2\r\n\"\r\nL2C1;\"\r\nL2C2\"";

        var target1 = sourceTable.ToCsvString();

        Assert.Equal(target1, expected.Replace(";", TableHelper.CsvColumnSeparator.ToString()));

        var target2 = expected.ParseTable(';');

        Assert.Equal(sourceTable.Length, target2.Count);
        Assert.True(sourceTable[0].SequenceEqual(target2[0]));
        Assert.True(sourceTable[1].SequenceEqual(target2[1]));
    }

    [Fact]
    public void TableHelper_ParseSimpleColumnWithEmptyLines()
    {
        const string input = """
                             Line1
                             Line2

                             Line4
                             """;

        var result = input.ParseTable('\t');

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Equal(4, result.Sum(r => r.Count));

        Assert.True(result.Select(r => r[0]).SequenceEqual(new[] { "Line1", "Line2", "", "Line4" }));
    }
}
