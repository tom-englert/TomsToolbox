namespace TomsToolbox.Composition.Tests;

using System.Composition;

internal interface IInterface1
{
    string Name { get; }
}

internal interface IInterface2
{
    string Name { get; }
}

[Export(typeof(IInterface1))]
internal class ExportedWithoutContract1 : IInterface1
{
    public string Name => "NoContract";
}

[Export("Contract", typeof(IInterface1))]
internal class ExportedWithContract1 : IInterface1
{
    public string Name => "Contract";
}

[Export("Contract", typeof(IInterface2))]
internal class ExportedWithContract2 : IInterface2
{
    public string Name => "Contract";
}

[Export(typeof(IInterface2))]
internal class ExportedWithoutContract2 : IInterface2
{
    public string Name => "NoContract";
}
