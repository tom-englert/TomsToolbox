namespace TomsToolbox.Core.Tests
{
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RegexExtensionsTest
    {
        [TestMethod]
        public void RegexExtensions_Split_RoundTrip_Test()
        {
            const string source = "This is  a text with    varying \twhite space";
            const string expected = source;

            var regex = new Regex(@"\s+");
            var fragments = regex.Split(source, (value, isMatch) => value).ToArray();

            Assert.AreEqual(15, fragments.Length);

            var result = string.Concat(fragments);


            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RegexExtensions_Split_DecorateUnmatched_Test()
        {
            const string source = "This is  a text with    varying \twhite space";
            const string expected = "<This> <is>  <a> <text> <with>    <varying> \t<white> <space>";

            var regex = new Regex(@"\s+");
            var fragments = regex.Split(source, (value, isMatch) => isMatch ? value : $"<{value}>").ToArray();

            Assert.AreEqual(15, fragments.Length);

            var result = string.Concat(fragments);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RegexExtensions_Split_ReplaceMatched_Test()
        {
            const string source = "This is  a text with    varying \twhite space";
            const string expected = "This-is-a-text-with-varying-white-space";

            var regex = new Regex(@"\s+");
            var fragments = regex.Split(source, (value, isMatch) => isMatch ? "-" : value).ToArray();

            Assert.AreEqual(15, fragments.Length);

            var result = string.Concat(fragments);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RegexExtensions_Split_ReplaceInStrings_Test()
        {
            // replace 'this' with 'that', but only inside quoted strings.
            const string source
                = "this is code with strings embedded in single quotes. 'this is a string' but this is code between, 'and this is another string' and here again is this is code.";
            const string expected
                = "this is code with strings embedded in single quotes. 'that is a string' but this is code between, 'and that is another string' and here again is this is code.";

            var regex = new Regex(@"'.*?'");

            var fragments = regex.Split(source, (value, isMatch) => isMatch ? value.Replace("this", "that") : value).ToArray();

            var result = string.Concat(fragments);

            Assert.AreEqual(expected, result);
        }

    }
}
