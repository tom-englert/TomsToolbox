// ReSharper disable All
namespace TomsToolbox.Desktop.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Wpf;

    [TestClass]
    public class ClipboardHelperTests
    {
        [TestMethod]
        public void ClipboardHelper_ParseQuotedStrings()
        {
            const string sourceText = "\"\"\"$(SolutionDir).nuget\\nuget.exe\"\" pack \"\"$(ProjectPath)\"\" -OutputDirectory \"\"$(SolutionDir)..\\Bin\\Deploy\\.\"\" -IncludeReferencedProjects -Prop Configuration=$(ConfigurationName)\"";
            const string targetText = "\"$(SolutionDir).nuget\\nuget.exe\" pack \"$(ProjectPath)\" -OutputDirectory \"$(SolutionDir)..\\Bin\\Deploy\\.\" -IncludeReferencedProjects -Prop Configuration=$(ConfigurationName)";
            var table = ClipboardHelper.ParseTable(sourceText, '\t');

            Assert.AreEqual(1, table.Count);
            Assert.AreEqual(1, table[0].Count);
            Assert.AreEqual(targetText, table[0][0]);
        }

        [TestMethod]
        public void ClipboardHelper_QuotedStringsWithLineBreaks_RoundTrip()
        {
            var sourceTable = new[] { new[] { "L1\r\nC1", "L1C2\r\n" }, new[] { "L2C1", "\r\nL2C2" } };
            var expected = "\"L1\r\nC1\";\"L1C2\r\n\"\r\nL2C1;\"\r\nL2C2\"";

            var target1 = sourceTable.ToCsvString();

            Assert.AreEqual(target1, expected.Replace(";", ClipboardHelper.CsvColumnSeparator.ToString()));

            var target2 = ClipboardHelper.ParseTable(expected, ';');

            Assert.AreEqual(sourceTable.Length, target2.Count);
            Assert.IsTrue(sourceTable[0].SequenceEqual(target2[0]));
            Assert.IsTrue(sourceTable[1].SequenceEqual(target2[1]));
        }
    }
}
