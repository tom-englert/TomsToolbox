using System.Reflection;

[assembly: AssemblyCompany("tom-englert.de")]
[assembly: AssemblyProduct("Tom's Toolbox")]
[assembly: AssemblyCopyright("© tom-englert.de 2015-2016")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion(Product.Version)]
[assembly: AssemblyFileVersion(Product.Version)]

internal static class Product
{
    public const string Version = "1.0.49.0";
}
