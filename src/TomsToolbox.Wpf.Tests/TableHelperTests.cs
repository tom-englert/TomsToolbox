#nullable disable
namespace TomsToolbox.Wpf.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TableHelperTests
    {
        [TestMethod]
        public void TableHelper_ParseQuotedStrings()
        {
            const string sourceText = "\"\"\"$(SolutionDir).nuget\\nuget.exe\"\" pack \"\"$(ProjectPath)\"\" -OutputDirectory \"\"$(SolutionDir)..\\Bin\\Deploy\\.\"\" -IncludeReferencedProjects -Prop Configuration=$(ConfigurationName)\"";
            const string targetText = "\"$(SolutionDir).nuget\\nuget.exe\" pack \"$(ProjectPath)\" -OutputDirectory \"$(SolutionDir)..\\Bin\\Deploy\\.\" -IncludeReferencedProjects -Prop Configuration=$(ConfigurationName)";
            var table = sourceText.ParseTable('\t');

            Assert.IsNotNull(table);
            Assert.AreEqual(1, table.Count);
            Assert.AreEqual(1, table[0].Count);
            Assert.AreEqual(targetText, table[0][0]);
        }

        [TestMethod]
        public void TableHelper_QuotedStringsWithLineBreaks_RoundTrip()
        {
            IList<string> line1 = new[] { "L1\r\nC1", "L1C2\r\n" };
            IList<string> line2 = new[] { "L2C1", "\r\nL2C2" };
            var sourceTable = new[] { line1, line2 };
            var expected = "\"L1\r\nC1\";\"L1C2\r\n\"\r\nL2C1;\"\r\nL2C2\"";

            var target1 = sourceTable.ToCsvString();

            Assert.AreEqual(target1, expected.Replace(";", TableHelper.CsvColumnSeparator.ToString()));

            var target2 = expected.ParseTable(';');

            Assert.AreEqual(sourceTable.Length, target2.Count);
            Assert.IsTrue(sourceTable[0].SequenceEqual(target2[0]));
            Assert.IsTrue(sourceTable[1].SequenceEqual(target2[1]));
        }
    }
}
