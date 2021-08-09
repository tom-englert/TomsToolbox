// ReSharper disable UnusedVariable
namespace TomsToolbox.ObservableCollections.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Summary description for ObservableCompositeCollectionTest
    /// </summary>
    [TestClass]
    public class ObservableCompositeCollectionTest
    {
        [TestMethod]
        public void ObservableCompositeCollection_FillWithoutEventHandlersTest()
        {
            var collection = new ObservableCollection<Item>();
            var compositeCollection = new ObservableCompositeCollection<Item>();

            CommpareAdd(collection, compositeCollection);
        }

        [TestMethod]
        public void ObservableCompositeCollection_FillWithEventHandlersTest()
        {
            var collection = new ObservableCollection<Item>();
            var compositeCollection = new ObservableCompositeCollection<Item>();

            var collectionEvents = 0;
            var compositeEvents = 0;

            collection.CollectionChanged += (_, _) => collectionEvents += 1;
            compositeCollection.CollectionChanged += (_, _) => compositeEvents += 1;

            collection.CollectionChanged += Collection_CollectionChanged;
            compositeCollection.CollectionChanged += Collection_CollectionChanged;

            CommpareAdd(collection, compositeCollection);

            Assert.AreEqual(collectionEvents, collection.Count);
            Assert.AreEqual(compositeEvents, compositeCollection.Count);
        }

        [TestMethod]
        public void ObservableCompositeCollection_ModifyTest()
        {
            var data = CreateData();

            var compositeCollection = new ObservableCompositeCollection<Item>();

            compositeCollection.Content.AddRange(data);
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));

            // move
            data[99].Move(0, 999);
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));
            // replace
            data[1][42] = new Item { Index = -1 };
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));
            // add
            data[42].AddRange(new[] { new Item(), new Item(), new Item() });
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));
            // remove
            data[218].RemoveWhere(item => (item.Index & 1) == 0);
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));
            // reset
            data[698].Clear();
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));
            // modify sources
            data.RemoveAt(777);
            compositeCollection.Content.RemoveAt(777);
            Assert.IsTrue(compositeCollection.SequenceEqual(data.SelectMany(list => list)));
        }

        private static void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // this is what the ItemsCollection of an ItemsControl does...
            var item = ((IList)sender)[e.NewStartingIndex];
        }

        private static void CommpareAdd(ObservableCollection<Item> collection, ObservableCompositeCollection<Item> compositeCollection)
        {
            var data = CreateData();

            var stopwatch = Stopwatch.StartNew();

            var t1 = stopwatch.Elapsed;

            collection.AddRange(data.SelectMany(list => list));

            var t2 = stopwatch.Elapsed;
            compositeCollection.Content.AddRange(data);

            var t3 = stopwatch.Elapsed;

            stopwatch.Stop();

            var compositeTime = t3 - t2;
            var plainTime = t2 - t1;
            var factor = compositeTime.TotalSeconds / plainTime.TotalSeconds;

            Debug.WriteLine("composite: {0} / plain {1} = {2}", compositeTime, plainTime, factor);
            Debug.WriteLine("{0} elements.", compositeCollection.Count);

            Assert.IsTrue(factor < 2);
            Assert.IsTrue(collection.SequenceEqual(compositeCollection));
        }

        private static IList<ObservableCollection<Item>> CreateData()
        {
            return Enumerable.Range(0, 1000).Select(i1 => new ObservableCollection<Item>(Enumerable.Range(0, 1000).Select(i2 => new Item { Index = 1000 * i1 + i2 }))).ToList();
        }

        class Item
        {
            public int Index { get; set; }
        }
    }
}
