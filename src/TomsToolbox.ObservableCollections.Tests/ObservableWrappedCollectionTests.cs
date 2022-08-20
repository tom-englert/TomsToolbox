// ReSharper disable UnusedVariable
#nullable disable
namespace TomsToolbox.ObservableCollections.Tests;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Xunit;

public class ObservableWrappedCollectionTests : IDisposable
{
    private readonly Random _random;
    private readonly ObservableCollection<string> _source;
    private readonly ObservableWrappedCollection<string, StringWrapper> _target;

    public ObservableWrappedCollectionTests()
    {
        _random = new Random(DateTime.Today.Day); // reproducible random sequence generating identical values at the same day.
        _source = new ObservableCollection<string>(_sourceStrings);
        _target = new ObservableWrappedCollection<string, StringWrapper>(_source, (s) => new StringWrapper(s));
    }

    public void Dispose()
    {
        // Nice for debugging...
        var result = string.Join("/", _target.Select(item => item.ToString()));
    }

    private class StringWrapper
    {
        public StringWrapper(string wrapped)
        {
            Wrapped = wrapped;
        }

        public string Wrapped
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return "$" + Wrapped + "$";
        }
    }

    private readonly string[] _sourceStrings = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();

    [Fact]
    public void ObservableWrappedCollection_ConstructorFailTest()
    {
        Assert.ThrowsAny<Exception>(() =>
        {
            var collection = new ObservableWrappedCollection<int, int>(null, null);
        });
    }

    [Fact]
    public void ObservableWrappedCollection_ConstructorTest()
    {
        VerifyConsistency();
    }

    [Fact]
    public void ObservableWrappedCollection_RemoveTest()
    {
        while (_source.Count > 0)
        {
            _source.RemoveAt(_random.Next(_source.Count));
            VerifyConsistency();
        }
    }

    [Fact]
    public void ObservableWrappedCollection_InsertTest()
    {
        foreach (var newValue in Enumerable.Range(0, 9).Select(i => "new" + i.ToString()))
        {
            _source.Insert(_random.Next(_source.Count + 1), newValue);
            VerifyConsistency();
        }
    }

    [Fact]
    public void ObservableWrappedCollection_MoveTest()
    {
        for (var i = 0; i < 10; i++)
        {
            _source.Move(_random.Next(_source.Count), _random.Next(_source.Count));
            VerifyConsistency();
        }
    }

    private void VerifyConsistency()
    {
        Assert.Equal(_source.Count, _target.Count);
        Assert.True(_source.SequenceEqual(_target.Select(item => item.Wrapped)));
    }
}
