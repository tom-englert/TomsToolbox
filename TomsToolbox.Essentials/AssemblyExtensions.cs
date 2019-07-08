namespace TomsToolbox.Essentials
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    using JetBrains.Annotations;

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
        [NotNull]
        public static DirectoryInfo GetAssemblyDirectory([NotNull] this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;

            var assemblyLocation = Path.GetDirectoryName(new Uri(codeBase).LocalPath) 
                                   ?? throw new InvalidOperationException("Can't evaluate assembly code base: " + codeBase);

            return new DirectoryInfo(assemblyLocation);
        }

        /// <summary>
        /// Gets the directory in which the given assembly is stored.
        /// </summary>
        /// <param name="assemblyName">The assembly.</param>
        /// <returns>The directory in which the given assembly is stored.</returns>
        [NotNull]
        public static DirectoryInfo GetAssemblyDirectory([NotNull] this AssemblyName assemblyName)
        {
            var codeBase = assemblyName.CodeBase;

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
        [NotNull]
        public static Uri GeneratePackUri([NotNull] this Assembly assembly)
        {
            var name = new AssemblyName(assembly.FullName).Name;

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
        [NotNull]
        public static Uri GeneratePackUri([NotNull] this Assembly assembly, [NotNull] string relativeUri)
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
        [NotNull]
        public static Uri GeneratePackUri([NotNull] this Assembly assembly, [NotNull] Uri relativeUri)
        {
            return new Uri(assembly.GeneratePackUri(), relativeUri);
        }
    }
}
