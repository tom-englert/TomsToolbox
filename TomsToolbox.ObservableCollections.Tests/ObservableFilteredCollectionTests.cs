namespace TomsToolbox.ObservableCollections.Tests
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Core;
    using TomsToolbox.Desktop;

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

        [TestMethod]
        public void ObservableFilteredCollection_LiveFilteringTest()
        {
            var source = new ObservableCollection<TestObject>(Enumerable.Range(0, 10).Select(i => new TestObject(i)));
            var target = new ObservableFilteredCollection<TestObject>(source, s => (s.Value & 1) != 0, "Value");

            NotifyCollectionChangedEventArgs lastEventArgs = null;
            NotifyCollectionChangedEventHandler callback = (_, e) => lastEventArgs = e;

            target.CollectionChanged += callback;

            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 1, 3, 5, 7, 9 }));

            source.RemoveRange(o => o.Value == 2);
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 1, 3, 5, 7, 9 }));
            Assert.IsNull(lastEventArgs);

            source.RemoveRange(o => o.Value == 3);
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 1, 5, 7, 9 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastEventArgs.Action);
            Assert.AreEqual(3, lastEventArgs.OldItems.Cast<TestObject>().Single().Value);
            lastEventArgs = null;

            source.Single(o => o.Value == 5).Value = 6;
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 1, 7, 9 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, lastEventArgs.Action);
            Assert.AreEqual(6, lastEventArgs.OldItems.Cast<TestObject>().Single().Value);
            lastEventArgs = null;

            source.First(o => o.Value == 6).Value = 5;
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 1, 7, 9, 5 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(5, lastEventArgs.NewItems.Cast<TestObject>().Single().Value);
            lastEventArgs = null;
        }

        private class TestObject : ObservableObject
        {
            private int _value;

            public TestObject(int value)
            {
                _value = value;
            }

            public int Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    SetProperty(ref _value, value, nameof(Value));
                }
            }
        }
    }
}
