namespace TomsToolbox.Essentials.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

public class AutoWeakIndexerTest
{
    [Fact]
    public async Task AutoWeakIndexer_WithReferenceTest()
    {
        var indexer = new AutoWeakIndexer<int, ICollection<int>>(_ => new List<int>());

        var list = indexer[0];

        GC.Collect();

        await Task.Run(() =>
        {
            indexer[0].Add(1);
        });

        GC.Collect();

        await Task.Run(() =>
        {
            indexer[0].Add(1);
        });

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task AutoWeakIndexer_WithoutReferenceTest()
    {
        var indexer = new AutoWeakIndexer<int, ICollection<int>>(_ => new List<int>());

        await Task.Run(() =>
        {
            indexer[0].Add(1);
        });

        GC.Collect();

        await Task.Run(() =>
        {
            indexer[0].Add(1);
        });

        var list = indexer[0];

        Assert.Equal(1, list.Count);
    }
}
