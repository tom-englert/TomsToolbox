namespace TomsToolbox.Core.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AutoWeakIndexerTest
    {
        [TestMethod]
        public void AutoWeakIndexer_WithReferenceTest()
        {
            var indexer = new AutoWeakIndexer<int, ICollection<int>>(_ => new List<int>());

            var list = indexer[0];

            GC.Collect();

            indexer[0].Add(1);

            GC.Collect();

            indexer[0].Add(1);

            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void AutoWeakIndexer_WithoutReferenceTest()
        {
            var indexer = new AutoWeakIndexer<int, ICollection<int>>(_ => new List<int>());

            indexer[0].Add(1);

            GC.Collect();

            indexer[0].Add(1);

            var list = indexer[0];

            Assert.AreEqual(1, list.Count);
        }
    }
}
