namespace TomsToolbox.ObservableCollections.Tests
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ObservableIndexerTests
    {
        private const string AnotherString = "another string";

        [TestMethod]
        public void ObservableIndexer_SetTest()
        {
            var target = new ObservableIndexer<int, string>(i => (i + 1).ToString());

            Assert.AreEqual("2", target[1]);
            Assert.AreEqual("4", target[3]);
            Assert.AreEqual("7", target[6]);

            Assert.IsTrue(target.Select(item => item.Key).SequenceEqual(new[] { 1, 3, 6 }));
            Assert.IsTrue(target.Select(item => item.Value).SequenceEqual(new[] { "2", "4", "7" }));

            target[5] = AnotherString;

            Assert.IsTrue(target.Select(item => item.Key).SequenceEqual(new[] { 1, 3, 6, 5 }));
            Assert.IsTrue(target.Select(item => item.Value).SequenceEqual(new[] { "2", "4", "7", AnotherString }));

            Assert.AreEqual("2", target[1]);
            Assert.AreEqual("4", target[3]);
            Assert.AreEqual("7", target[6]);
            Assert.AreEqual(AnotherString, target[5]);
        }

        [TestMethod]
        public void ObservableIndexer_RemoveTest()
        {
            var target = new ObservableIndexer<int, string>(i => (i + 1).ToString());

            Assert.AreEqual("2", target[1]);
            Assert.AreEqual("4", target[3]);
            Assert.AreEqual("7", target[6]);

            Assert.IsTrue(target.Select(item => item.Key).SequenceEqual(new[] { 1, 3, 6 }));
            Assert.IsTrue(target.Select(item => item.Value).SequenceEqual(new[] { "2", "4", "7" }));

            target.Remove(3);

            Assert.IsTrue(target.Select(item => item.Key).SequenceEqual(new[] { 1, 6 }));
            Assert.IsTrue(target.Select(item => item.Value).SequenceEqual(new[] { "2", "7" }));

            Assert.AreEqual("2", target[1]);
            Assert.AreEqual("4", target[3]);
            Assert.AreEqual("7", target[6]);
        }
    }
}
