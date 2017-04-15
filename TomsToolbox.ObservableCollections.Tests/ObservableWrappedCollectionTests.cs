namespace TomsToolbox.ObservableCollections.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using JetBrains.Annotations;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ObservableWrappedCollectionTests
    {
        private Random _random;
        private ObservableCollection<string> _source;
        private ObservableWrappedCollection<string, StringWrapper> _target;

        [TestInitialize]
        public void TestInitialize()
        {
            _random = new Random(DateTime.Today.Day); // reproducible random sequence generating identical values at the same day.
            _source = new ObservableCollection<string>(_sourceStrings);
            _target = new ObservableWrappedCollection<string, StringWrapper>(_source, (s) => new StringWrapper(s));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Nice for debugging...
            var result = string.Join("/", _target.Select(item => item.ToString()));
        }

        private class StringWrapper
        {
            public StringWrapper([CanBeNull] string wrapped)
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

        [NotNull] [ItemNotNull]
        private readonly string[] _sourceStrings = Enumerable.Range(0, 10).Select(i => i.ToString()).ToArray();

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void ObservableWrappedCollection_ConstructorFailTest()
        {
            new ObservableWrappedCollection<int, int>(null, null);
        }

        [TestMethod]
        public void ObservableWrappedCollection_ConstructorTest()
        {
            VerifyConsistency();
        }

        [TestMethod]
        public void ObservableWrappedCollection_RemoveTest()
        {
            while (_source.Count > 0)
            {
                _source.RemoveAt(_random.Next(_source.Count));
                VerifyConsistency();
            }
        }

        [TestMethod]
        public void ObservableWrappedCollection_InsertTest()
        {
            foreach (var newValue in Enumerable.Range(0, 9).Select(i => "new" + i.ToString()))
            {
                _source.Insert(_random.Next(_source.Count + 1), newValue);
                VerifyConsistency();
            }
        }

        [TestMethod]
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
            Assert.AreEqual(_source.Count, _target.Count);
            Assert.IsTrue(_source.SequenceEqual(_target.Select(item => item.Wrapped)));
        }
    }
}
