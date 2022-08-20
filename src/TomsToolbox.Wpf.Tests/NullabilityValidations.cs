// ReSharper disable All
namespace TomsToolbox.Wpf.Tests;

using System.Windows.Threading;

class NullabilityValidations
{
    void Test()
    {
        var x = ((Dispatcher)null!).Invoke(MethodWithNullableReturn);
        var x1 = x?.ToString();

        var y = ((Dispatcher)null!).Invoke(MethodWithNotNullableReturn);
        var y1 = y.ToString();
    }

    object? MethodWithNullableReturn()
    {
        return null;
    }

    object MethodWithNotNullableReturn()
    {
        return new object();
    }
}