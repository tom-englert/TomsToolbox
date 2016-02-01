namespace TomsToolbox.ObservableCollections.Tests
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ObservableFilteredCollectionTests
    {
        [TestMethod]
        public void ObservableFilteredCollection_AddRemoveTest()
        {
            var source = new ObservableCollection<int>(Enumerable.Range(0, 10));
            var target = new ObservableFilteredCollection<int>(source, s => (s & 1) != 0);

            NotifyCollectionChangedEventArgs lastEventArgs = null;

            NotifyCollectionChangedEventHandler callback = (_, e) => lastEventArgs = e;
            
            target.CollectionChanged += callback;

            source.Remove(2);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 3, 5, 7, 9 }));
            Assert.IsNull(lastEventArgs);

            source.Remove(3);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 5, 7, 9 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastEventArgs.Action);
            Assert.AreEqual(3, lastEventArgs.OldItems[0]);
            lastEventArgs = null;

            source.Remove(4);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 5, 7, 9 }));
            Assert.IsNull(lastEventArgs);

            source.Remove(5);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 7, 9 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastEventArgs.Action);
            Assert.AreEqual(5, lastEventArgs.OldItems[0]);
            lastEventArgs = null;

            source.Add(4);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 7, 9 }));
            Assert.IsNull(lastEventArgs);

            source.Add(5);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 7, 9, 5 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(5, lastEventArgs.NewItems[0]);
            lastEventArgs = null;

            source.Insert(2, 5);
            Assert.IsTrue(target.SequenceEqual(new[] { 1, 7, 9, 5, 5 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(5, lastEventArgs.NewItems[0]);
        }
    }
}
