namespace TomsToolbox.Essentials;

using System;
using System.Globalization;
using System.IO;
using System.Reflection;

/// <summary>
/// Extension methods for assemblies.
/// </summary>
public static class AssemblyExtensions
{

    /// <summary>
    /// Gets the directory in which the given assembly is stored.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>The directory in which the given assembly is stored.</returns>
    public static DirectoryInfo GetAssemblyDirectory(this Assembly assembly)
    {
        var location = assembly.Location;

        var assemblyLocation = Path.GetDirectoryName(location) 
                               ?? throw new InvalidOperationException("Can't evaluate assembly code base: " + location);

        return new DirectoryInfo(assemblyLocation);
    }

    /// <summary>
    /// Gets the directory in which the given assembly is stored.
    /// </summary>
    /// <param name="assemblyName">The assembly.</param>
    /// <returns>The directory in which the given assembly is stored.</returns>
    [Obsolete("AssemblyName is not fully supported in DotNet")]
    public static DirectoryInfo GetAssemblyDirectory(this AssemblyName assemblyName)
    {
        var codeBase = assemblyName.CodeBase ?? throw new InvalidOperationException("Can't evaluate assembly code base: " + assemblyName);
        var assemblyLocation = Path.GetDirectoryName(new Uri(codeBase).LocalPath) 
                               ?? throw new InvalidOperationException("Can't evaluate assembly code base: " + codeBase);

        return new DirectoryInfo(assemblyLocation);
    }

    /// <summary>
    /// Generates the pack URI according to <see href="https://msdn.microsoft.com/library/aa970069.aspx"/> for the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <returns>The pack URI.</returns>
    /// <remarks>
    /// The URI is in the format "pack://application:,,,/ReferencedAssembly;component/"
    /// </remarks>
    public static Uri GeneratePackUri(this Assembly assembly)
    {
        var assemblyFullName = assembly.FullName ?? throw new ArgumentException("Assembly does not have a full name", nameof(assembly));
        var name = new AssemblyName(assemblyFullName).Name;

        return new Uri(string.Format(CultureInfo.InvariantCulture, "pack://application:,,,/{0};component/", name), UriKind.Absolute);
    }

    /// <summary>
    /// Generates the pack URI according to <see href="https://msdn.microsoft.com/library/aa970069.aspx" /> for the resource in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <param name="relativeUri">The relative URI of the resource.</param>
    /// <returns>
    /// The pack URI.
    /// </returns>
    /// <remarks>
    /// The URI is in the format "pack://application:,,,/ReferencedAssembly;component/RelativeUri"
    /// </remarks>
    public static Uri GeneratePackUri(this Assembly assembly, string relativeUri)
    {
        return assembly.GeneratePackUri(new Uri(relativeUri, UriKind.Relative));
    }

    /// <summary>
    /// Generates the pack URI according to <see href="https://msdn.microsoft.com/library/aa970069.aspx" /> for the resource in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly containing the resource.</param>
    /// <param name="relativeUri">The relative URI of the resource.</param>
    /// <returns>
    /// The pack URI.
    /// </returns>
    /// <remarks>
    /// The URI is in the format "pack://application:,,,/ReferencedAssembly;component/RelativeUri"
    /// </remarks>
    public static Uri GeneratePackUri(this Assembly assembly, Uri relativeUri)
    {
        return new Uri(assembly.GeneratePackUri(), relativeUri);
    }
}
