namespace TomsToolbox.Desktop.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.ObservableCollections;

    [TestClass]
    public class RelayedEventAttributeTests
    {
        class GoverningClass1 : ObservableObject
        {
            private int _value;

            public int Value
            {
                get
                {
                    return _value;
                }
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
                get
                {
                    return _value;
                }
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
                get
                {
                    return _otherValue;
                }
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
            private readonly GoverningClass2 _governingClass2;

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
            public int Value
            {
                get
                {
                    return _governingClass1.Value;
                }
            }

            [RelayedEvent(typeof(GoverningClass2), "OtherValue")]
            public int MyOtherValue
            {
                get
                {
                    return _governingClass2.OtherValue;
                }
            }
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
            public int MyValue
            {
                get
                {
                    return _governingClass1.Value;
                }
            }
        }

        [TestMethod]
        public void RelayedEventAttribute_SingleGoverningClassTest()
        {
            var receivedEvents = new ObservableIndexer<string, int>(_ => 0);
            var governing = new GoverningClass1();
            var relaying = new RelayingClass(governing);

            relaying.PropertyChanged += (sender, e) => receivedEvents[e.PropertyName] += 1;

            governing.Value = 5;

            Assert.AreEqual(5, relaying.Value);
            Assert.AreEqual(1, receivedEvents.Count);
            Assert.AreEqual(1, receivedEvents["Value"]);

            governing.Value = 7;

            Assert.AreEqual(7, relaying.Value);
            Assert.AreEqual(1, receivedEvents.Count);
            Assert.AreEqual(2, receivedEvents["Value"]);
        }

        [TestMethod]
        public void RelayedEventAttribute_MultipleGoverningClassTest()
        {
            var receivedEvents = new ObservableIndexer<string, int>(_ => 0);
            var governing1 = new GoverningClass1();
            var governing2 = new GoverningClass2();
            var relaying = new RelayingClass(governing1, governing2);

            relaying.PropertyChanged += (sender, e) => receivedEvents[e.PropertyName] += 1;

            governing1.Value = 5;

            Assert.AreEqual(5, relaying.Value);
            Assert.AreEqual(0, relaying.MyOtherValue);
            Assert.AreEqual(1, receivedEvents.Count);
            Assert.AreEqual(1, receivedEvents["Value"]);

            governing1.Value = 7;

            Assert.AreEqual(7, relaying.Value);
            Assert.AreEqual(0, relaying.MyOtherValue);
            Assert.AreEqual(1, receivedEvents.Count);
            Assert.AreEqual(2, receivedEvents["Value"]);

            // Governing2.Value is not relayed, changes should not generate relayed events
            governing2.Value = 8;

            Assert.AreEqual(7, relaying.Value);
            Assert.AreEqual(0, relaying.MyOtherValue);
            Assert.AreEqual(1, receivedEvents.Count);
            Assert.AreEqual(2, receivedEvents["Value"]);

            governing2.OtherValue = 8;

            Assert.AreEqual(7, relaying.Value);
            Assert.AreEqual(8, relaying.MyOtherValue);
            Assert.AreEqual(2, receivedEvents.Count);
            Assert.AreEqual(2, receivedEvents["Value"]);
            Assert.AreEqual(1, receivedEvents["MyOtherValue"]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RelayedEventAttribute_CallingRelayEventsOfWithoutRelayedEventAttributeTest()
        {
            var governing = new GoverningClass1();
            var relaying = new BadRelayingClass1(governing);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RelayedEventAttribute_CallingRelayEventsOfWithInvalidRelayedEventAttributeTest()
        {
            var governing = new GoverningClass1();
            var relaying = new BadRelayingClass2(governing);
        }
    }
}
