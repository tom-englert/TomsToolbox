namespace TomsToolbox.Wpf.Tests.Converters;

using System.Windows.Data;

using Xunit;

using TomsToolbox.Essentials;
using TomsToolbox.Wpf.Converters;

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

    [Fact]
    public void ObjectToTextConverter_Static_Enum()
    {
        Assert.Equal("This is item 1", ObjectToTextConverter.Convert("key1", Items.Item1));
        Assert.Equal("This is other text on item 1", ObjectToTextConverter.Convert("key2", Items.Item1));
        Assert.Equal("This is item 2", ObjectToTextConverter.Convert("key1", Items.Item2));
    }

    [Fact]
    public void ObjectToTextConverter_Dynamic_Class()
    {
        IValueConverter target = new ObjectToTextConverter { Key = "key" };

        Assert.Equal("This is a class", target.Convert(new Class(), null, null, null));
    }

    [Fact]
    public void ObjectToTextConverter_Dynamic_Class_BadKey()
    {
        IValueConverter target = new ObjectToTextConverter { Key = "keyxx" };

        Assert.Equal("TomsToolbox.Wpf.Tests.Converters.ObjectToTextConverterTests+Class", target.Convert(new Class(), null, null, null));
    }

    [Fact]
    public void ObjectToTextConverter_Dynamic_Class_ParameterOverride()
    {
        IValueConverter target = new ObjectToTextConverter { Key = "keyxx" };

        Assert.Equal("This is a class", target.Convert(new Class(), null, "key", null));
    }
}