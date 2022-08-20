// ReSharper disable PossibleNullReferenceException
// ReSharper disable AssignNullToNotNullAttribute
#nullable disable
namespace TomsToolbox.ObservableCollections.Tests;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Xunit;

using TomsToolbox.Essentials;

public class ObservableExtensionsTests
{
    class TestObject : INotifyPropertyChanged
    {
        private IList<int> _inner;
        private int _value;

        public IList<int> Inner
        {
            get => _inner;
            set
            {
                if (Equals(_inner, value))
                    return;

                _inner = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Inner)));
            }
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

    [Fact]
    public void ObservableSelect_AddItemsTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelect(item => item.Value);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedValues = new[] { 1, 2, 3 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Add, 3) };

        var testObject1 = new TestObject { Value = 1 };
        var testObject2 = new TestObject { Value = 2 };
        var testObject3 = new TestObject { Value = 3 };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        values.CollectionChanged += collectionChangedEventHandler;
        collection.AddRange(testObject1, testObject2, testObject3);

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelect_ChangeItemsTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelect(item => item.Value);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedValues = new[] { 2, 3, 4 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Replace, 3) };

        var testObject1 = new TestObject { Value = 1 };
        var testObject2 = new TestObject { Value = 2 };
        var testObject3 = new TestObject { Value = 3 };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        collection.AddRange(testObject1, testObject2, testObject3);

        values.CollectionChanged += collectionChangedEventHandler;

        foreach (var item in collection)
        {
            item.Value += 1;
        }

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelect_RemoveItemsTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelect(item => item.Value);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var initialValues = new[] { 1, 2, 3 };
        var expectedValues = new[] { 1, 3 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Remove, 1) };

        var testObject1 = new TestObject { Value = 1 };
        var testObject2 = new TestObject { Value = 2 };
        var testObject3 = new TestObject { Value = 3 };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        collection.AddRange(testObject1, testObject2, testObject3);
        Assert.True(values.SequenceEqual(initialValues));

        values.CollectionChanged += collectionChangedEventHandler;

        collection.RemoveAt(1);

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelectMany_AddItemsTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelectMany(item => item.Inner);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedValues = new[] { 1, 2, 3, 1, 2, 3, 1, 2, 3 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Add, 9) };

        var testObject1 = new TestObject { Inner = new ObservableCollection<int>() };
        var testObject2 = new TestObject { Inner = new ObservableCollection<int>() };
        var testObject3 = new TestObject { Inner = new ObservableCollection<int>() };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        values.CollectionChanged += collectionChangedEventHandler;

        Assert.Equal(0, values.Count);

        collection.AddRange(testObject1, testObject2, testObject3);

        foreach (var item in collection)
        {
            item.Inner.AddRange(1, 2, 3);
        }

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelectMany_ChangeItemValuesTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelectMany(item => item.Inner);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedInitialValues = new[] { 1, 2, 3, 1, 2, 3, 1, 2, 3 };
        var expectedValues = new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Replace, 9) };

        var testObject1 = new TestObject { Inner = new ObservableCollection<int>(new[] { 1, 2, 3 }) };
        var testObject2 = new TestObject { Inner = new ObservableCollection<int>(new[] { 1, 2, 3 }) };
        var testObject3 = new TestObject { Inner = new ObservableCollection<int>(new[] { 1, 2, 3 }) };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        collection.AddRange(testObject1, testObject2, testObject3);
        Assert.True(values.SequenceEqual(expectedInitialValues));

        values.CollectionChanged += collectionChangedEventHandler;

        var innerValue = 1;

        foreach (var item in collection)
        {
            var subItems = item.Inner;
            for (var i = 0; i < subItems.Count; i++)
            {
                subItems[i] = ++innerValue;
            }
        }

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelectMany_ReplaceItemsTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelectMany(item => item.Inner);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedInitialValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var expectedValues = new[] { 1, 2, 3, 10, 20, 30, 7, 8, 9 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Remove, 3), KeyValuePair(NotifyCollectionChangedAction.Add, 3) };

        var testObject1 = new TestObject { Inner = new ObservableCollection<int>(new[] { 1, 2, 3 }) };
        var testObject2 = new TestObject { Inner = new ObservableCollection<int>(new[] { 4, 5, 6 }) };
        var testObject3 = new TestObject { Inner = new ObservableCollection<int>(new[] { 7, 8, 9 }) };

        var newInnerCollection = new ObservableCollection<int>(new[] { 10, 20, 30 });

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        collection.AddRange(testObject1, testObject2, testObject3);
        Assert.True(values.SequenceEqual(expectedInitialValues));

        values.CollectionChanged += collectionChangedEventHandler;

        collection[1].Inner = newInnerCollection;

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelectMany_RemoveItemsTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelectMany(item => item.Inner);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedInitialValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var expectedValues = new[] { 1, 2, 3, 7, 8, 9 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Remove, 3) };

        var testObject1 = new TestObject { Inner = new ObservableCollection<int>(new[] { 1, 2, 3 }) };
        var testObject2 = new TestObject { Inner = new ObservableCollection<int>(new[] { 4, 5, 6 }) };
        var testObject3 = new TestObject { Inner = new ObservableCollection<int>(new[] { 7, 8, 9 }) };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        collection.AddRange(testObject1, testObject2, testObject3);
        Assert.True(values.SequenceEqual(expectedInitialValues));

        values.CollectionChanged += collectionChangedEventHandler;

        collection.RemoveAt(1);

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableSelectMany_RemoveValuesTest()
    {
        var collection = new ObservableCollection<TestObject>();
        var values = collection.ObservableSelectMany(item => item.Inner);
        var receivedEvents = new ObservableIndexer<NotifyCollectionChangedAction, int>(_ => 0);

        var expectedInitialValues = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var expectedValues = new[] { 1, 3, 4, 6, 7, 9 };
        var expectedEvents = new[] { KeyValuePair(NotifyCollectionChangedAction.Remove, 3) };

        var testObject1 = new TestObject { Inner = new ObservableCollection<int>(new[] { 1, 2, 3 }) };
        var testObject2 = new TestObject { Inner = new ObservableCollection<int>(new[] { 4, 5, 6 }) };
        var testObject3 = new TestObject { Inner = new ObservableCollection<int>(new[] { 7, 8, 9 }) };

        NotifyCollectionChangedEventHandler collectionChangedEventHandler = (_, e) =>
        {
            receivedEvents[e.Action] += 1;
        };

        collection.AddRange(testObject1, testObject2, testObject3);
        Assert.True(values.SequenceEqual(expectedInitialValues));

        values.CollectionChanged += collectionChangedEventHandler;

        foreach (var item in collection)
        {
            item.Inner.RemoveAt(1);
        }

        Assert.True(receivedEvents.SequenceEqual(expectedEvents));
        Assert.True(values.SequenceEqual(expectedValues));
    }

    [Fact]
    public void ObservableWhere_AddRemoveTest()
    {
        var source = new ObservableCollection<int>(Enumerable.Range(0, 10));
        var target = source.ObservableWhere(s => (s & 1) != 0);

        NotifyCollectionChangedEventArgs lastEventArgs = null;

        NotifyCollectionChangedEventHandler callback = (_, e) => lastEventArgs = e;

        target.CollectionChanged += callback;

        source.Remove(2);
        Assert.True(target.SequenceEqual(new[] { 1, 3, 5, 7, 9 }));
        Assert.Null(lastEventArgs);

        source.Remove(3);
        Assert.True(target.SequenceEqual(new[] { 1, 5, 7, 9 }));
        Assert.Equal(NotifyCollectionChangedAction.Remove, lastEventArgs.Action);
        Assert.Equal(3, lastEventArgs.OldItems[0]);
        lastEventArgs = null;

        source.Remove(4);
        Assert.True(target.SequenceEqual(new[] { 1, 5, 7, 9 }));
        Assert.Null(lastEventArgs);

        source.Remove(5);
        Assert.True(target.SequenceEqual(new[] { 1, 7, 9 }));
        Assert.Equal(NotifyCollectionChangedAction.Remove, lastEventArgs.Action);
        Assert.Equal(5, lastEventArgs.OldItems[0]);
        lastEventArgs = null;

        source.Add(4);
        Assert.True(target.SequenceEqual(new[] { 1, 7, 9 }));
        Assert.Null(lastEventArgs);

        source.Add(5);
        Assert.True(target.SequenceEqual(new[] { 1, 7, 9, 5 }));
        Assert.Equal(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
        Assert.Equal(5, lastEventArgs.NewItems[0]);
        lastEventArgs = null;

        source.Insert(2, 5);
        Assert.True(target.SequenceEqual(new[] { 1, 7, 9, 5, 5 }));
        Assert.Equal(NotifyCollectionChangedAction.Add, lastEventArgs.Action);
        Assert.Equal(5, lastEventArgs.NewItems[0]);
    }


    private static KeyValuePair<NotifyCollectionChangedAction, int> KeyValuePair(NotifyCollectionChangedAction action, int value)
    {
        return new KeyValuePair<NotifyCollectionChangedAction, int>(action, value);
    }

}