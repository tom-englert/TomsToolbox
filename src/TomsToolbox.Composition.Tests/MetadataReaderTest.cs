namespace TomsToolbox.Composition.Tests
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using ApprovalTests;
    using ApprovalTests.Reporters;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    [TestClass]
    [UseReporter(typeof(DiffReporter))]
    public class MetadataReaderTest
    {
        private static readonly Regex _versionRegex = new Regex(@"Version=2\.\d+\.\d+\.\d+");

        [TestMethod]
        public void ReadSampleAppTest()
        {
            var assembly = typeof(SampleApp.Mef1.App).Assembly;
            var result = MetadataReader.Read(assembly);

            var data = Serialize(result);

            Approvals.VerifyJson(data);
        }

        [TestMethod]
        public void ReadSampleAppMef2Test()
        {
            var assembly = typeof(SampleApp.MainWindow).Assembly;
            var result = MetadataReader.Read(assembly);

            var data = Serialize(result);

            Approvals.VerifyJson(data);
        }

        private static string Serialize(IList<ExportInfo> result)
        {
            return _versionRegex.Replace(JsonConvert.SerializeObject(result), "Version=2.0.0.0");
        }
    }
}
