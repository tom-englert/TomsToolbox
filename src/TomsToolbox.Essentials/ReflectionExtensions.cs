namespace TomsToolbox.Essentials;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

/// <summary>
/// Methods to ease reflection
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets all types in all assemblies, including nested types.
    /// </summary>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>The types in all assemblies.</returns>
    public static IEnumerable<Type> EnumerateAllTypes(this IEnumerable<Assembly?> assemblies)
    {
        return assemblies.SelectMany(EnumerateAllTypes);
    }

    /// <summary>
    /// Gets all types in the assembly, including nested types.
    /// </summary>
    /// <param name="assembly">The assembly. If assembly is null, an empty list is returned.</param>
    /// <returns>The types in the assembly.</returns>
    public static IEnumerable<Type> EnumerateAllTypes(this Assembly? assembly)
    {
        return assembly?.GetTypes().SelectMany(GetSelfAndNestedTypes) ?? Enumerable.Empty<Type>();
    }

    private static IEnumerable<Type> GetSelfAndNestedTypes(Type? type)
    {
        return type == null ? Enumerable.Empty<Type>() : new[] { type }.Concat(type.GetNestedTypes().SelectMany(GetSelfAndNestedTypes));
    }

    /// <summary>
    /// Enumerates all types in all assemblies with .dll extension in the specified directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <returns>All types in all assemblies in the specified directory</returns>
    public static IEnumerable<Type> EnumerateAllTypes(this DirectoryInfo directory)
    {
        return EnumerateAllTypes(directory, "*.dll");
    }

    /// <summary>
    /// Enumerates all types in all assemblies in the specified directory.
    /// </summary>
    /// <param name="directory">The directory.</param>
    /// <param name="searchPattern">The search string. The default pattern is "*", which returns all files.</param>
    /// <returns>All types in all assemblies in the specified directory</returns>
    public static IEnumerable<Type> EnumerateAllTypes(this DirectoryInfo directory, string searchPattern)
    {
        var assemblyFiles = directory.EnumerateFiles(searchPattern);

        return assemblyFiles.Select(TryLoadAssembly).EnumerateAllTypes();
    }

    /// <summary>
    /// Tries to load the assembly from the specified file without generating exceptions.
    /// </summary>
    /// <param name="assemblyFile">The assembly file.</param>
    /// <returns>The assembly if the assembly could be loaded; otherwise <c>null</c>.</returns>
    public static Assembly? TryLoadAssembly(this FileSystemInfo? assemblyFile)
    {
        if (assemblyFile == null)
            return null;

        try
        {
            var fullName = assemblyFile.FullName;
            return Assembly.LoadFile(fullName);
        }
        catch
        {
            // there are various different exceptions that can happen here, not really predictable
        }

        return null;
    }

    /// <summary>
    /// Tries to load the assembly from the specified file without generating exceptions.
    /// </summary>
    /// <param name="assemblyFile">The assembly file.</param>
    /// <returns>The assembly if the assembly could be loaded; otherwise <c>null</c>.</returns>
    public static Assembly? TryLoadAssemblyForReflectionOnly(this FileSystemInfo? assemblyFile)
    {
        if (assemblyFile == null)
            return null;

        try
        {
            return Assembly.ReflectionOnlyLoadFrom(assemblyFile.FullName);
        }
        catch
        {
            // there are various different exceptions that can happen here, not really predictable
        }

        return null;
    }
}