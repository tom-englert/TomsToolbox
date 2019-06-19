namespace TomsToolbox.Desktop.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Reflection;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Extension methods to simplify access of the composition host.
    /// </summary>
    public static class CompositionExtensions
    {
        /// <summary>
        /// Adds a new catalog to the container.
        /// </summary>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="catalog">The catalog.</param>
        public static void AddCatalog([NotNull] this ICompositionHost compositionHost, [NotNull] ComposablePartCatalog catalog)
        {
            // ReSharper disable once PossibleNullReferenceException
            compositionHost.Catalog.Catalogs.Add(catalog);
        }

        /// <summary>
        /// Adds a new assembly catalog to the containers aggregate catalog.
        /// </summary>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="assemblies">The assemblies to add.</param>
        public static void AddCatalog([NotNull] this ICompositionHost compositionHost, [NotNull, ItemCanBeNull] params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                if (assembly == null)
                    continue;

                if (assembly.ReflectionOnly)
                    throw new InvalidOperationException("Assemblies in the catalog must not be loaded for reflection only.");

                compositionHost.AddCatalog(new AssemblyCatalog(assembly));
            }
        }

        /// <summary>
        /// Adds a new type catalog to the containers aggregate catalog.
        /// </summary>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="types">The types to add.</param>
        public static void AddCatalog([NotNull] this ICompositionHost compositionHost, [NotNull, ItemNotNull] params Type[] types)
        {
            compositionHost.AddCatalog(new TypeCatalog(types));
        }

        /// <summary>
        /// Returns the exported object with the contract name derived from the specified
        /// type parameter. If there is not exactly one matching exported object, an
        /// exception is thrown.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the exported object to return. The contract name is also derived
        /// from this type parameter.
        /// </typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <returns>
        /// The exported object with the contract name derived from the specified type
        /// parameter.
        /// </returns>
        /// <exception cref="System.ComponentModel.Composition.ImportCardinalityMismatchException">
        /// There are zero exported objects with the contract name derived from T in
        /// the System.ComponentModel.Composition.Hosting.CompositionContainer.-or-There
        /// is more than one exported object with the contract name derived from T in
        /// the System.ComponentModel.Composition.Hosting.CompositionContainer.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.ComponentModel.Composition.Hosting.CompositionContainer object
        /// has been disposed of.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionContractMismatchException">
        /// The underlying exported object cannot be cast to T.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionException">
        /// An error occurred during composition. System.ComponentModel.Composition.CompositionException.Errors
        /// will contain a collection of errors that occurred.
        /// </exception>
        [NotNull]
        public static T GetExportedValue<T>([NotNull] this ICompositionHost compositionHost) where T : class
        {
            // ReSharper disable once AssignNullToNotNullAttribute => GetExportedValue never returns null.
            return compositionHost.Container.GetExportedValue<T>();
        }

        /// <summary>
        /// Returns the exported object with the contract name derived from the specified
        /// type parameter. If there is not exactly one matching exported object, an
        /// exception is thrown.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the exported object to return. The contract name is also derived
        /// from this type parameter.
        /// </typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="contractName">The contract name of the exported object to return, or null or an empty string ("") to use the default contract name.</param>
        /// <returns>
        /// The exported object with the contract name derived from the specified type
        /// parameter.
        /// </returns>
        /// <exception cref="System.ComponentModel.Composition.ImportCardinalityMismatchException">
        /// There are zero exported objects with the contract name derived from T in
        /// the System.ComponentModel.Composition.Hosting.CompositionContainer.-or-There
        /// is more than one exported object with the contract name derived from T in
        /// the System.ComponentModel.Composition.Hosting.CompositionContainer.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.ComponentModel.Composition.Hosting.CompositionContainer object
        /// has been disposed of.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionContractMismatchException">
        /// The underlying exported object cannot be cast to T.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionException">
        /// An error occurred during composition. System.ComponentModel.Composition.CompositionException.Errors
        /// will contain a collection of errors that occurred.
        /// </exception>
        [NotNull]
        // ReSharper disable once AnnotateNotNullParameter
        public static T GetExportedValue<T>([NotNull] this ICompositionHost compositionHost, [CanBeNull] string contractName) where T : class
        {
            // ReSharper disable AssignNullToNotNullAttribute => null is a legal parameter!
            return compositionHost.Container.GetExportedValue<T>(contractName);
            // ReSharper restor AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets the exported object with the contract name derived from the specified type parameter
        /// or the default value for the specified type, or throws an exception if there is
        /// more than one matching exported object.
        /// </summary>
        /// <typeparam name="T">The type of the exported object to return. The contract name is also derived
        /// from this type parameter.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="contractName">The contract name of the exported object to return, or null or an empty string ("") to use the default contract name.</param>
        /// <returns>
        /// The exported object with the contract name derived from T, if found; otherwise, the default value for T.
        /// </returns>
        /// <exception cref="System.ComponentModel.Composition.ImportCardinalityMismatchException">
        /// There  is more than one exported object with the contract name derived from T in
        /// the System.ComponentModel.Composition.Hosting.CompositionContainer.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.ComponentModel.Composition.Hosting.CompositionContainer object
        /// has been disposed of.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionContractMismatchException">
        /// The underlying exported object cannot be cast to T.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionException">
        /// An error occurred during composition. System.ComponentModel.Composition.CompositionException.Errors
        /// will contain a collection of errors that occurred.
        /// </exception>
        // ReSharper disable once AnnotateNotNullParameter
        [CanBeNull]
        public static T GetExportedValueOrDefault<T>([NotNull] this ICompositionHost compositionHost, [CanBeNull] string contractName) where T : class
        {
            return compositionHost.Container.GetExportedValueOrDefault<T>(contractName);
        }

        /// <summary>
        /// Gets the exported object with the contract name derived from the specified type parameter
        /// or the default value for the specified type, or throws an exception if there is
        /// more than one matching exported object.
        /// </summary>
        /// <typeparam name="T">The type of the exported object to return. The contract name is also derived
        /// from this type parameter.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <returns>
        /// The exported object with the contract name derived from T, if found; otherwise, the default value for T.
        /// </returns>
        /// <exception cref="System.ComponentModel.Composition.ImportCardinalityMismatchException">
        /// There  is more than one exported object with the contract name derived from T in
        /// the System.ComponentModel.Composition.Hosting.CompositionContainer.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.ComponentModel.Composition.Hosting.CompositionContainer object
        /// has been disposed of.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionContractMismatchException">
        /// The underlying exported object cannot be cast to T.
        /// </exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionException">
        /// An error occurred during composition. System.ComponentModel.Composition.CompositionException.Errors
        /// will contain a collection of errors that occurred.
        /// </exception>
        [CanBeNull]
        public static T GetExportedValueOrDefault<T>([NotNull] this ICompositionHost compositionHost) where T : class
        {
            return compositionHost.Container.GetExportedValueOrDefault<T>();
        }

        /// <summary>
        /// Creates a part from the specified value and composes it in the specified composition container. See <see cref="AttributedModelServices.ComposeExportedValue{T}(CompositionContainer, T)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the new part.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="exportedValue">The value to compose.</param>
        public static void ComposeExportedValue<T>([NotNull] this ICompositionHost compositionHost, [CanBeNull] T exportedValue)
        {
            compositionHost.Container.ComposeExportedValue(exportedValue);
        }

        /// <summary>
        /// Composes the specified part, with recomposition and validation disabled.
        /// </summary>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="attributedPart">The part to compose.</param>
        public static void SatisfyImportsOnce([NotNull] this ICompositionHost compositionHost, [NotNull] object attributedPart)
        {
            compositionHost.Container.SatisfyImportsOnce(attributedPart);
        }

        /// <summary>
        /// Gets all the exported objects with the contract name derived from the specified type parameter.
        /// </summary>
        /// <typeparam name="T">The type of the exported object to return. The contract name is also derived from this type parameter.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <returns>
        /// The exported objects with the contract name derived from the specified type parameter, if found; otherwise, an empty <see cref="T:System.Collections.ObjectModel.Collection`1" /> object.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> object has been disposed of.</exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionContractMismatchException">One or more of the underlying exported objects cannot be cast to <typeparamref name="T" />.</exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionException">An error occurred during composition. <see cref="P:System.ComponentModel.Composition.CompositionException.Errors" /> will contain a collection of errors that occurred.</exception>
        [NotNull, ItemNotNull]
        public static IEnumerable<T> GetExportedValues<T>([NotNull] this ICompositionHost compositionHost)
        {
            return compositionHost.Container.GetExportedValues<T>();
        }

        /// <summary>
        /// Gets all the exported objects with the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type of the exported object to return.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="contractName">The contract name of the exported objects to return; or null or an empty string ("") to use the default contract name.</param>
        /// <returns>
        /// The exported objects with the specified contract name, if found; otherwise, an empty <see cref="T:System.Collections.ObjectModel.Collection`1" /> object.
        /// </returns>
        /// <exception cref="System.ObjectDisposedException">The <see cref="T:System.ComponentModel.Composition.Hosting.CompositionContainer" /> object has been disposed of.</exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionContractMismatchException">One or more of the underlying exported values cannot be cast to <typeparamref name="T" />.</exception>
        /// <exception cref="System.ComponentModel.Composition.CompositionException">An error occurred during composition. <see cref="P:System.ComponentModel.Composition.CompositionException.Errors" /> will contain a collection of errors that occurred.</exception>
        [NotNull, ItemNotNull]
        // ReSharper disable once AnnotateNotNullParameter
        public static IEnumerable<T> GetExportedValues<T>([NotNull] this ICompositionHost compositionHost, [CanBeNull] string contractName)
        {
            return compositionHost.Container.GetExportedValues<T>(contractName);
        }

        /// <summary>
        /// Gets all the exports.
        /// </summary>
        /// <typeparam name="T">The type parameter of the System.Lazy{T} objects to return.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <returns>
        /// The System.Lazy{T} objects, if found; otherwise, an empty System.Collections.Generic.IEnumerable{T} object.
        /// </returns>
        [NotNull, ItemNotNull]
        public static IEnumerable<Lazy<T>> GetExports<T>([NotNull] this ICompositionHost compositionHost)
        {
            return compositionHost.Container.GetExports<T>();
        }

        /// <summary>
        /// Gets all the exports with the specified contract name.
        /// </summary>
        /// <typeparam name="T">The type parameter of the System.Lazy{T} objects to return.</typeparam>
        /// <param name="compositionHost">The composition host.</param>
        /// <param name="contractName">The contract name of the System.Lazy{T} objects to return, or null or an empty string ("") to use the default contract name.</param>
        /// <returns>The System.Lazy{T} objects with the specified contract name, if found; otherwise, an empty System.Collections.Generic.IEnumerable{T} object.</returns>
        [NotNull, ItemNotNull]
        // ReSharper disable once AnnotateNotNullParameter
        public static IEnumerable<Lazy<T>> GetExports<T>([NotNull] this ICompositionHost compositionHost, [CanBeNull] string contractName)
        {
            return compositionHost.Container.GetExports<T>(contractName);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>
        /// A service object of type <typeparamref name="T"/>, or null if there is no service object of type <typeparamref name="T"/>.
        /// </returns>
        [CanBeNull]
        public static T GetService<T>([NotNull] this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService(typeof(T)).SafeCast<T>();
        }
    }
}
