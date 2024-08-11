namespace TomsToolbox.Composition.Tests;

public class MetadataAdapterTest
{
    [Fact]
    public void MetadataAdapterTest1()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["Name"] = "Tom",
            ["Age"] = 18
        };

        var target = new MetadataAdapter(metadata);

        Assert.Equal("Tom", target.GetValue("Name"));
        Assert.Equal(18, target.GetValue("Age"));
        Assert.True(target.TryGetValue("Name", out var name));
        Assert.Equal("Tom", name);
        Assert.True(target.TryGetValue("Age", out var age));
        Assert.Equal(18, age);
        Assert.Throws<KeyNotFoundException>(() => target.GetValue("Missing"));
    }

    interface IMetadata
    {
        string Name { get; }
        int Age { get; }
        string WrongType { get; }
        double Missing { get; }
    }

    [Fact]
    public void MetadataAdapterTest2()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["Name"] = "Tom",
            ["Age"] = 18,
            ["WrongType"] = 42
        };

        var target = MetadataAdapter.Create<IMetadata>(new MetadataAdapter(metadata));

        Assert.Equal("Tom", target.Name);
        Assert.Equal(18, target.Age);
        Assert.Equal("42", target.WrongType);
        Assert.Equal(0.0, target.Missing);
    }
}
