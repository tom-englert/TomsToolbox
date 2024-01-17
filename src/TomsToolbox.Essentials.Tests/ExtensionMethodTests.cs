// ReSharper disable RedundantUsingDirective
namespace TomsToolbox.Essentials.Tests
{
    using Xunit;
    using System.Linq;
    using TomsToolbox.Essentials;

    public class ExtensionMethodTests
    {
        [Fact]
        public void ToDictionaryDoesNotConflictWithNet8()
        {
            var input = (IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string> { { "1", "2" } };

            var output = input.ToDictionary();

            Assert.NotNull(output);
            Assert.Single(output);
            Assert.Equal("2", output["1"]);
        }

        [Fact]
        public void ToDictionaryWithComparerDoesNotConflictWithNet8()
        {
            var input = (IEnumerable<KeyValuePair<string, string>>)new Dictionary<string, string> { { "1", "2" } };

            var output = input.ToDictionary(StringComparer.Ordinal);

            Assert.NotNull(output);
            Assert.Single(output);
            Assert.Equal("2", output["1"]);
        }
    }
}
