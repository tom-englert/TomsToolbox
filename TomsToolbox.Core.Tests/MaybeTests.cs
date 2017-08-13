// ReSharper disable PossibleNullReferenceException
// ReSharper disable InconsistentNaming
namespace TomsToolbox.Core.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MaybeTests
    {
        private const string C1P1 = "Property C1";
        private const string C2F1 = "Function C2";
        private static readonly string[] C2P1 = {"1", "2", "3", "4"};
        private const string C2P2 = "Property C2";

        [TestMethod]
        public void Maybe_SimplePropertyTest()
        {
            var source = new C1();
            var target = source.Maybe().Return(x => x.P1);
            Assert.AreEqual(C1P1, target);
        }

        [TestMethod]
        public void Maybe_NestedPropertyTest()
        {
            var source = new C1();
            var target = source.Maybe().Return(x => x.P2.P1);
            Assert.AreEqual(C2P1, target);
        }

        [TestMethod]
        public void Maybe_NumericIndexerTest()
        {
            var source = new C1();
            var target = source.Maybe(x => x.P2).Return(x => x.P1[2]);
            Assert.AreEqual("3", target);
        }

        [TestMethod]
        public void Maybe_StringIndexerTest()
        {
            var source = new C1();
            var target = source.Maybe().Return(x => x.P2["test"]);
            Assert.AreEqual(4, target);
        }

        [TestMethod]
        public void Maybe_NestedFunctionTest()
        {
            var source = new C1();
            var target = source.Maybe().Select(x => x.Func()).Return(x => x.Func());
            Assert.AreEqual(C2F1, target);
        }

        [TestMethod]
        public void Maybe_NestedPropertyNullTest()
        {
            var source = new C1();

            const string defaultValue = "Dummy";

            // ensure both are different!
            Assert.AreNotEqual(defaultValue, C2P2);

            // ensure default behavior
            var target = source.Maybe().Select(x => x.P2).Return(x => x.P2, defaultValue);
            Assert.AreEqual(C2P2, target);

            // now ensure fall back to default value
            source.P2 = null;
            target = source.Maybe().Select(x => x.P2).Return(x => x.P2, defaultValue);
            Assert.AreEqual(defaultValue, target);

            source = null;
            target = source.Maybe().Select(x => x.P2).Return(x => x.P2, defaultValue);
            Assert.AreEqual(defaultValue, target);
        }


        private class C1
        {
            public C1()
            {
                P1 = C1P1;
                P2 = new C2();
            }

            public string P1
            {
                get;
                set;
            }

            public C2 P2
            {
                get;
                set;
            }

            public C2 Func()
            {
                return new C2();
            }

            public int this[int index] => index + 1;
        }

        private class C2
        {
            public C2()
            {
                P1 = C2P1;
                P2 = C2P2;
            }

            public string[] P1
            {
                get;
                set;
            }

            public string P2
            {
                get;
                set;
            }

            public string Func()
            {
                return C2F1;
            }

            public int this[string index] => index.Length;
        }
    }
}
