namespace TomsToolbox.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

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
        [NotNull, ItemNotNull]
        public static IEnumerable<Type> EnumerateAllTypes([NotNull, ItemCanBeNull] this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(EnumerateAllTypes);
        }

        /// <summary>
        /// Gets all types in the assembly, including nested types.
        /// </summary>
        /// <param name="assembly">The assembly. If assmbly is null, an empty list is returned.</param>
        /// <returns>The types in the assembly.</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<Type> EnumerateAllTypes([CanBeNull] this Assembly assembly)
        {
            return assembly?.GetTypes().SelectMany(GetSelfAndNestedTypes) ?? Enumerable.Empty<Type>();
        }

        [NotNull, ItemNotNull]
        private static IEnumerable<Type> GetSelfAndNestedTypes([CanBeNull] Type type)
        {
            return type == null ? Enumerable.Empty<Type>() : new[] { type }.Concat(type.GetNestedTypes().SelectMany(GetSelfAndNestedTypes));
        }

        /// <summary>
        /// Enumerates all types in all assemblies with .dll extension in the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>All types in all assemblies in the specified directory</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<Type> EnumerateAllTypes([NotNull] this DirectoryInfo directory)
        {
            return EnumerateAllTypes(directory, "*.dll");
        }

        /// <summary>
        /// Enumerates all types in all assemblies in the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchPattern">The search string. The default pattern is "*", which returns all files.</param>
        /// <returns>All types in all assemblies in the specified directory</returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<Type> EnumerateAllTypes([NotNull] this DirectoryInfo directory, [NotNull] string searchPattern)
        {
            var assemblyFiles = directory.EnumerateFiles(searchPattern);

            return assemblyFiles.Select(TryLoadAssembly).EnumerateAllTypes();
        }

        /// <summary>
        /// Tries to load the assembly from the specified file without generating exceptions.
        /// </summary>
        /// <param name="assemblyFile">The assembly file.</param>
        /// <returns>The assembly if the assembly could be loaded; otherwise <c>null</c>.</returns>
        [CanBeNull]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
        public static Assembly TryLoadAssembly([CanBeNull] this FileSystemInfo assemblyFile)
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
        [CanBeNull]
        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
        public static Assembly TryLoadAssemblyForReflectionOnly([CanBeNull] this FileSystemInfo assemblyFile)
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

        /// <summary>
        /// Gets the directory in which the given assembly is stored.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The directory in which the given assembly is stored.</returns>
        [NotNull]
        public static DirectoryInfo GetAssemblyDirectory([NotNull] this Assembly assembly)
        {
            var assemblyLocation = Path.GetDirectoryName(assembly.Location);

            return new DirectoryInfo(assemblyLocation);
        }
    }
}
