namespace TomsToolbox.ObservableCollections.Tests;

using System.Linq;

using Xunit;

public class ObservableIndexerTests
{
    private const string AnotherString = "another string";

    [Fact]
    public void ObservableIndexer_SetTest()
    {
        var target = new ObservableIndexer<int, string>(i => (i + 1).ToString());

        Assert.Equal("2", target[1]);
        Assert.Equal("4", target[3]);
        Assert.Equal("7", target[6]);

        Assert.True(target.Select(item => item.Key).SequenceEqual(new[] { 1, 3, 6 }));
        Assert.True(target.Select(item => item.Value).SequenceEqual(new[] { "2", "4", "7" }));

        target[5] = AnotherString;

        Assert.True(target.Select(item => item.Key).SequenceEqual(new[] { 1, 3, 6, 5 }));
        Assert.True(target.Select(item => item.Value).SequenceEqual(new[] { "2", "4", "7", AnotherString }));

        Assert.Equal("2", target[1]);
        Assert.Equal("4", target[3]);
        Assert.Equal("7", target[6]);
        Assert.Equal(AnotherString, target[5]);
    }

    [Fact]
    public void ObservableIndexer_RemoveTest()
    {
        var target = new ObservableIndexer<int, string>(i => (i + 1).ToString());

        Assert.Equal("2", target[1]);
        Assert.Equal("4", target[3]);
        Assert.Equal("7", target[6]);

        Assert.True(target.Select(item => item.Key).SequenceEqual(new[] { 1, 3, 6 }));
        Assert.True(target.Select(item => item.Value).SequenceEqual(new[] { "2", "4", "7" }));

        target.Remove(3);

        Assert.True(target.Select(item => item.Key).SequenceEqual(new[] { 1, 6 }));
        Assert.True(target.Select(item => item.Value).SequenceEqual(new[] { "2", "7" }));

        Assert.Equal("2", target[1]);
        Assert.Equal("4", target[3]);
        Assert.Equal("7", target[6]);
    }
}
