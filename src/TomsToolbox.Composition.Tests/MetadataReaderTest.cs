namespace TomsToolbox.Composition.Tests;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using VerifyMSTest;

using VerifyTests;

[TestClass]
public class MetadataReaderTest : VerifyBase
{
    private static readonly Regex _versionRegex = new(@"Version=2\.\d+\.\d+\.\d+");

#if NETFRAMEWORK
        [TestMethod]
        public async Task ReadSampleAppTest()
        {
            var assembly = typeof(SampleApp.Mef1.App).Assembly;
            var result = MetadataReader.Read(assembly);

            var data = Serialize(result);

            await Verify(data);
        }

        [TestMethod]
        public async Task ReadSampleAppMef2Test()
        {
            var assembly = typeof(SampleApp.MainWindow).Assembly;
            var result = MetadataReader.Read(assembly);

            var data = Serialize(result);

            await Verify(data);
        }

        [TestMethod]
        public async Task ReadThisAssemblyTest()
        {
            var assembly = GetType().Assembly;
            var result = MetadataReader.Read(assembly);

            var data = Serialize(result);

            await Verify(data);
        }
#endif

    private static string Serialize(IList<ExportInfo> result)
    {
        return _versionRegex.Replace(JsonConvert.SerializeObject(result, Formatting.Indented), "Version=2.0.0.0");
    }
}