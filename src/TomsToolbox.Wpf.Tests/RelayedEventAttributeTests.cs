// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Local
namespace TomsToolbox.Wpf.Tests;

using System;

using Xunit;

using TomsToolbox.ObservableCollections;

public class RelayedEventAttributeTests
{
    class GoverningClass1 : ObservableObject
    {
        private int _value;

        public int Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnPropertyChanged(() => Value);
            }
        }
    }

    class GoverningClass2 : ObservableObject
    {
        private int _value;
        private int _otherValue;

        public int Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnPropertyChanged(() => Value);
            }
        }

        public int OtherValue
        {
            get => _otherValue;
            set
            {
                if (_otherValue == value)
                    return;

                _otherValue = value;
                OnPropertyChanged(() => OtherValue);
            }
        }
    }

    class RelayingClass : ObservableObject
    {
        private readonly GoverningClass1 _governingClass1;
        private readonly GoverningClass2? _governingClass2;

        public RelayingClass(GoverningClass1 governingClass)
        {
            _governingClass1 = governingClass;
            RelayEventsOf(governingClass);
        }

        public RelayingClass(GoverningClass1 governingClass1, GoverningClass2 governingClass2)
        {
            _governingClass1 = governingClass1;
            _governingClass2 = governingClass2;

            RelayEventsOf(governingClass1);
            RelayEventsOf(governingClass2);
        }

        [RelayedEvent(typeof(GoverningClass1))]
        public int Value => _governingClass1.Value;

        [RelayedEvent(typeof(GoverningClass2), "OtherValue")]
        public int MyOtherValue => _governingClass2?.OtherValue ?? int.MinValue;
    }

    class BadRelayingClass1 : ObservableObject
    {
        private readonly GoverningClass1 _governingClass;

        public BadRelayingClass1(GoverningClass1 governingClass)
        {
            _governingClass = governingClass;
            // Invalid initialization, class has no property with a RelayedEventAttribute for GoverningClass1
            RelayEventsOf(governingClass);
        }
    }

    class BadRelayingClass2 : ObservableObject
    {
        private readonly GoverningClass1 _governingClass1;

        public BadRelayingClass2(GoverningClass1 governingClass)
        {
            _governingClass1 = governingClass;
            RelayEventsOf(governingClass);
        }

        // Invalid attribute, GoverningClass1 does not have a property "MyValue".
        [RelayedEvent(typeof(GoverningClass1))]
        public int MyValue => _governingClass1.Value;
    }

    [Fact]
    public void RelayedEventAttribute_SingleGoverningClassTest()
    {
        var receivedEvents = new ObservableIndexer<string, int>(_ => 0);
        var governing = new GoverningClass1();
        var relaying = new RelayingClass(governing);

        relaying.PropertyChanged += (_, e) => receivedEvents[e.PropertyName] += 1;

        governing.Value = 5;

        Assert.Equal(5, relaying.Value);
        Assert.Single(receivedEvents);
        Assert.Equal(1, receivedEvents["Value"]);

        governing.Value = 7;

        Assert.Equal(7, relaying.Value);
        Assert.Single(receivedEvents);
        Assert.Equal(2, receivedEvents["Value"]);
    }

    [Fact]
    public void RelayedEventAttribute_MultipleGoverningClassTest()
    {
        var receivedEvents = new ObservableIndexer<string, int>(_ => 0);
        var governing1 = new GoverningClass1();
        var governing2 = new GoverningClass2();
        var relaying = new RelayingClass(governing1, governing2);

        relaying.PropertyChanged += (_, e) => receivedEvents[e.PropertyName] += 1;

        governing1.Value = 5;

        Assert.Equal(5, relaying.Value);
        Assert.Equal(0, relaying.MyOtherValue);
        Assert.Single(receivedEvents);
        Assert.Equal(1, receivedEvents["Value"]);

        governing1.Value = 7;

        Assert.Equal(7, relaying.Value);
        Assert.Equal(0, relaying.MyOtherValue);
        Assert.Single(receivedEvents);
        Assert.Equal(2, receivedEvents["Value"]);

        // Governing2.Value is not relayed, changes should not generate relayed events
        governing2.Value = 8;

        Assert.Equal(7, relaying.Value);
        Assert.Equal(0, relaying.MyOtherValue);
        Assert.Single(receivedEvents);
        Assert.Equal(2, receivedEvents["Value"]);

        governing2.OtherValue = 8;

        Assert.Equal(7, relaying.Value);
        Assert.Equal(8, relaying.MyOtherValue);
        Assert.Equal(2, receivedEvents.Count);
        Assert.Equal(2, receivedEvents["Value"]);
        Assert.Equal(1, receivedEvents["MyOtherValue"]);
    }
    [Fact]
    public void RelayedEventAttribute_MultipleGoverningWeakReferenceGetsReleasedTest()
    {
        var receivedEvents = new ObservableIndexer<string, int>(_ => 0);
        var governing1 = new GoverningClass1();
        var governing2 = new GoverningClass2();
        var relaying = new RelayingClass(governing1, governing2);

        relaying.PropertyChanged += (_, e) => receivedEvents[e.PropertyName] += 1;

        governing1.Value = 5;

        Assert.Equal(5, relaying.Value);
        Assert.Equal(0, relaying.MyOtherValue);
        Assert.Single(receivedEvents);
        Assert.Equal(1, receivedEvents["Value"]);

        governing2.OtherValue = 8;

        Assert.Equal(5, relaying.Value);
        Assert.Equal(8, relaying.MyOtherValue);
        Assert.Equal(2, receivedEvents.Count);
        Assert.Equal(1, receivedEvents["Value"]);
        Assert.Equal(1, receivedEvents["MyOtherValue"]);

        GC.Collect();
        GC.WaitForPendingFinalizers();

        governing1.Value = 5;
        governing2.OtherValue = 8;

        Assert.Equal(2, receivedEvents.Count);
        Assert.Equal(1, receivedEvents["Value"]);
        Assert.Equal(1, receivedEvents["MyOtherValue"]);
    }

    [Fact]
    public void RelayedEventAttribute_CallingRelayEventsOfWithoutRelayedEventAttributeTest()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var governing = new GoverningClass1();
            var relaying = new BadRelayingClass1(governing);
        });
    }

    [Fact]
    public void RelayedEventAttribute_CallingRelayEventsOfWithInvalidRelayedEventAttributeTest()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var governing = new GoverningClass1();
            var relaying = new BadRelayingClass2(governing);
        });
    }
}
