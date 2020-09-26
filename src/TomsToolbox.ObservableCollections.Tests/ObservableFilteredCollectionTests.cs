// ReSharper disable PossibleNullReferenceException
#nullable disable
namespace TomsToolbox.ObservableCollections.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Essentials;

    [TestClass]
    public class ObservableFilteredCollectionTests
    {
        [TestMethod]
        public void ObservableFilteredCollection_AddRemoveTest()
        {
            var source = new ObservableCollection<int>(Enumerable.Range(0, 10));
            var target = new ObservableFilteredCollection<int>(source, s => (s & 1) != 0);

            NotifyCollectionChangedEventArgs lastEventArgs = null;
            target.CollectionChanged += (_, e) => lastEventArgs = e;

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

            lastEventArgs = null;

            source.Clear();
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastEventArgs.Action);
            Assert.AreEqual(0, target.Count);
            lastEventArgs = null;

            source.Add(11);
            Assert.IsTrue(target.SequenceEqual(new[] { 11 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(11, lastEventArgs.NewItems.Cast<int>().Single());
            lastEventArgs = null;

            source.Add(12);
            Assert.IsTrue(target.SequenceEqual(new[] { 11 }));
            Assert.AreEqual(null, lastEventArgs);
            lastEventArgs = null;

            source.Add(13);
            Assert.IsTrue(target.SequenceEqual(new[] { 11, 13 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(13, lastEventArgs.NewItems.Cast<int>().Single());
            lastEventArgs = null;

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

            source.RemoveWhere(o => o.Value == 2);
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 1, 3, 5, 7, 9 }));
            Assert.IsNull(lastEventArgs);

            source.RemoveWhere(o => o.Value == 3);
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

            source.Clear();
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, lastEventArgs.Action);
            Assert.AreEqual(0, target.Count);
            lastEventArgs = null;

            source.Add(new TestObject(11));
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 11 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(11, lastEventArgs.NewItems.Cast<TestObject>().Single().Value);
            lastEventArgs = null;

            source.Add(new TestObject(12));
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 11 }));
            Assert.AreEqual(null, lastEventArgs);
            lastEventArgs = null;

            source.Add(new TestObject(13));
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 11, 13 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(13, lastEventArgs.NewItems.Cast<TestObject>().Single().Value);
            lastEventArgs = null;

            source.First(o => o.Value == 12).Value = 15;
            Assert.IsTrue(target.Select(t => t.Value).SequenceEqual(new[] { 11, 13, 15 }));
            Assert.AreEqual(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
            Assert.AreEqual(15, lastEventArgs.NewItems.Cast<TestObject>().Single().Value);
            lastEventArgs = null;
        }

        [TestMethod]
        public void ObservableFilteredCollection_WeakRefTest()
        {
            var source = new ObservableCollection<TestObject>(Enumerable.Range(0, 10).Select(i => new TestObject(i)));

            var changeCount = 0;

            void Inner()
            {
                var target = new ObservableFilteredCollection<TestObject>(source, s => (s.Value & 1) != 0, "Value");

                target.CollectionChanged += (sender, args) => changeCount += 1;

                source.RemoveAt(4);
                source.RemoveAt(4);

                Assert.AreEqual(1, changeCount);
            }

            Inner();
            GCCollect();

            source.RemoveAt(4);
            source.RemoveAt(4);

            Assert.AreEqual(1, changeCount);
        }

        private static void GCCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCApproach();
        }

        class TestObject : INotifyPropertyChanged
        {
            private int _value;

            public TestObject(int value)
            {
                _value = value;
            }

            public int Value
            {
                get => _value;
                set
                {
                    if (_value == value)
                        return;

                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
