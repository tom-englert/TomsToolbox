namespace TomsToolbox.Wpf.Tests.Converters
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf.Converters;

    [TestClass]
    public class ObjectToTextConverterTests
    {
        enum Items
        {
            [Text("key2", "This is other text on item 1")]
            [Text("key1", "This is item 1")]
            Item1,
            [Text("key1", "This is item 2")]
            Item2
        }

        [Text("key", "This is a class")]
        class Class
        {
            
        }

        [TestMethod]
        public void ObjectToTextConverter_Static_Enum()
        {
            Assert.AreEqual("This is item 1", ObjectToTextConverter.Convert("key1", Items.Item1));
        }

        [TestMethod]
        public void ObjectToTextConverter_Dynamic_Class()
        {
            var target = new ObjectToTextConverter { Key = "key" };

            Assert.AreEqual("This is a class", target.Convert(new Class(), null, null, null));
        }

        [TestMethod]
        public void ObjectToTextConverter_Dynamic_Class_BadKey()
        {
            var target = new ObjectToTextConverter { Key = "keyxx" };

            Assert.AreEqual("TomsToolbox.Wpf.Tests.Converters.ObjectToTextConverterTests+Class", target.Convert(new Class(), null, null, null));
        }

        [TestMethod]
        public void ObjectToTextConverter_Dynamic_Class_ParameterOverride()
        {
            var target = new ObjectToTextConverter { Key = "keyxx" };

            Assert.AreEqual("This is a class", target.Convert(new Class(), null, "key", null));
        }
    }
}
