namespace TomsToolbox.Composition.Tests;

using System.Collections.Generic;
using System.ComponentModel;

using global::Ninject;

using Xunit;

using TomsToolbox.Composition.Ninject;

using Xunit.Sdk;

public class NinjectTests : INotifyPropertyChanged
{
    [Fact]
    public void NamedObjectsCanNotBeRetrievedWithoutNameIfBindingIsNotUnique()
    {
        IKernel kernel = new StandardKernel();

        kernel.Bind(typeof(NinjectTests)).ToSelf().InSingletonScope().Named("A1");
        kernel.Bind(typeof(NinjectTests)).ToSelf().InSingletonScope().Named("A2");
        kernel.Bind(typeof(NinjectTests)).ToSelf().Named("B");

        var a1 = kernel.Get(typeof(NinjectTests), "A1");
        var a2 = kernel.Get(typeof(NinjectTests), "A2");
        var b = kernel.Get(typeof(NinjectTests), "B");

        Assert.NotEqual(a1, b);
        Assert.NotEqual(a1, a2);

        try
        {
            kernel.Get(typeof(NinjectTests));
            throw new XunitException("Should have thrown...");
        }
        catch (ActivationException)
        {
        }
    }

    [Fact]
    public void NamedObjectsCanBeRetrievedWithoutNameIfBindingIsUnique()
    {
        IKernel kernel = new StandardKernel();

        kernel.Bind(typeof(NinjectTests)).ToSelf().InSingletonScope().Named("A");

        var a = kernel.Get(typeof(NinjectTests), "A");
        var self = kernel.Get(typeof(NinjectTests));

        Assert.Equal(a, self);
    }

    [Fact]
    public void ExportsWithSingleNamedBindingsCanGetViaNameOrNative()
    {
        var exports = new[]
        {
            new ExportInfo
            {
                IsShared = true,
                Type = typeof(NinjectTests),
                Metadata = new IDictionary<string, object?>[]
                {
                    new Dictionary<string, object?>
                    {
                        {"ContractName", "A"},
                    },
                    new Dictionary<string, object?>
                    {
                        {"ContractType", typeof(NinjectTests)},
                        {"ContractName", "A"},
                    },
                    new Dictionary<string, object?>
                    {
                        {"ContractType", typeof(INotifyPropertyChanged)},
                        {"ContractName", "B"},
                    },
                }
            }
        };

        IKernel kernel = new StandardKernel();

        kernel.BindExports(exports);

        var self = kernel.Get<NinjectTests>();
        var a = kernel.Get<NinjectTests>("A");
        var b = kernel.Get<INotifyPropertyChanged>();
        var c = kernel.Get<INotifyPropertyChanged>("B");

        Assert.Equal(self, a);
        Assert.Equal(self, b);
        Assert.Equal(self, c);
    }

    [Fact]
    public void ExportsWithNativeAndSingleNamedBindingsCanGetViaNameOrNative()
    {
        var exports = new[]
        {
            new ExportInfo
            {
                IsShared = true,
                Type = typeof(NinjectTests),
                Metadata = new IDictionary<string, object?>[]
                {
                    new Dictionary<string, object?>(),
                    new Dictionary<string, object?>
                    {
                        {"ContractName", "A"},
                    },
                }
            }
        };

        IKernel kernel = new StandardKernel();

        kernel.BindExports(exports);

        var a = kernel.Get<NinjectTests>("A");
        var b = kernel.Get<NinjectTests>();

        Assert.Equal(a, b);
    }

    [Fact]
    public void ExportsWithNativeAndMultipleNamedBindingsCanGetViaNameOnly()
    {
        var exports = new[]
        {
            new ExportInfo
            {
                IsShared = true,
                Type = typeof(NinjectTests),
                Metadata = new IDictionary<string, object?>[]
                {
                    new Dictionary<string, object?>(),
                    new Dictionary<string, object?>
                    {
                        {"ContractName", "A"},
                    },
                    new Dictionary<string, object?>
                    {
                        {"ContractName", "B"},
                    },
                }
            }
        };

        IKernel kernel = new StandardKernel();

        kernel.BindExports(exports);

        var a = kernel.Get<NinjectTests>("A");
        var b = kernel.Get<NinjectTests>("B");

        Assert.Equal(a, b);

        try
        {
            kernel.Get<NinjectTests>();
            throw new XunitException("Should have thrown...");
        }
        catch (ActivationException)
        {
        }

    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}
