namespace TomsToolbox.Composition.Tests
{
    using ApprovalTests;
    using ApprovalTests.Reporters;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    [TestClass]
    [UseReporter(typeof(DiffReporter))]
    public class MetadataReaderTest
    {
        [TestMethod]
        public void ReadSampleAppTest()
        {
            var assembly = typeof(SampleApp.App).Assembly;
            var result = MetadataReader.Read(assembly);

            var data = JsonConvert.SerializeObject(result);

            Approvals.VerifyJson(data);
        }

        [TestMethod]
        public void ReadSampleAppMef2Test()
        {
            var assembly = typeof(SampleApp.Mef2.App).Assembly;
            var result = MetadataReader.Read(assembly);

            var data = JsonConvert.SerializeObject(result);

            Approvals.VerifyJson(data);
        }
    }
}
