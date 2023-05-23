namespace TomsToolbox.Wpf;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

using TomsToolbox.Essentials;

/// <summary>
/// Attribute to mark one property as dependent on another property.
/// If you call <see cref="ObservableObjectBase.OnPropertyChanged(string)"/> for one property, the property change event will also be raised for all dependent properties.
/// </summary>
/// <example><code language="C#"><![CDATA[
/// class X : ObservableObject
/// {
///     string Value { get { ... } }
///
///     [PropertyDependency("Value")]
///     int ValueLength { get { ... } }
///
///     void ChangeSomething()
///     {
///         OnPropertyChanged("Value");
///     }
/// }
/// ]]></code>
/// Calling 'OnPropertyChanged("Value")' will raise the PropertyChanged event for the "Value" property as well as for the dependent "ValueLength" property.
/// </example>
[AttributeUsage(AttributeTargets.Property)]
[CLSCompliant(false)]
public sealed class PropertyDependencyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyDependencyAttribute"/> class.
    /// </summary>
    /// <param name="propertyNames">The property names of the properties that this property depends on.</param>
    public PropertyDependencyAttribute([Localizable(false)] params string[] propertyNames)
    {
        PropertyNames = propertyNames;
    }

    /// <summary>
    /// Gets the names of the properties that the attributed property depends on.
    /// </summary>
    public IEnumerable<string> PropertyNames { get; }

    /// <summary>
    /// Creates the dependency mapping from the attributes of the properties of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A dictionary that maps the property names to all direct and indirect dependent property names.</returns>
    /// <exception cref="System.InvalidOperationException">Invalid dependency definitions, i.e. dependency to non-existing property.</exception>
    [return: NotNullIfNotNull(nameof(type))]
    public static Dictionary<string, IEnumerable<string>>? CreateDependencyMapping(Type? type)
    {
        if (type == null)
            return null;

        var properties = type.GetProperties();

        var dependencyDefinitions = properties
            .Select(prop => new
            {
                prop.Name,
                DependsUpon = prop.GetCustomAttributes<PropertyDependencyAttribute>(true).SelectMany(attr => attr.PropertyNames).ToArray()
            })
            .ToArray();

        var dependencySources = dependencyDefinitions
            .SelectMany(dependency => dependency.DependsUpon)
            .Distinct()
            .ToArray();

        var invalidDependencyDefinitions = dependencySources
            .Where(propertyName => !dependencyDefinitions.Select(d => d.Name).Contains(propertyName))
            .ToArray();

        if (invalidDependencyDefinitions.Any())
            throw new InvalidOperationException(@"Invalid dependency definitions: " + string.Join(", ", invalidDependencyDefinitions));

        var directDependencies = dependencySources.ToDictionary(
            source => source, 
            source => dependencyDefinitions
                .Where(dependency => dependency.DependsUpon.Contains(source))
                .Select(dependency => dependency.Name)
                .ToArray()
        );

        return directDependencies.Keys.ToDictionary(item => item, item => GetAllDependencies(item, directDependencies));
    }

    private static IEnumerable<string> GetAllDependencies(string item, IDictionary<string, string[]> directDependencies)
    {
        var allDependenciesAndSelf = new List<string> { item };

        for (var i = 0; i < allDependenciesAndSelf.Count; i++)
        {
            var key = allDependenciesAndSelf[i];
            if (!directDependencies.TryGetValue(key, out var indirectDependencies) || (indirectDependencies == null))
            {
                continue;
            }

            allDependenciesAndSelf.AddRange(indirectDependencies.Where(indirectDependency => !allDependenciesAndSelf.Contains(indirectDependency)));
        }

        return allDependenciesAndSelf.Skip(1).ToArray();
    }

    /// <summary>
    /// Gets a list of invalid dependency definitions in the entry types assembly and all referenced assemblies.
    /// </summary>
    /// <param name="entryType">Type of the entry.</param>
    /// <returns>A list of strings, each describing an invalid dependency definition. If no invalid definitions exist, the list is empty.</returns>
    /// <remarks>This method is mainly for writing unit test to detect invalid dependencies during compile time.</remarks>
    public static IEnumerable<string> GetInvalidDependencies(Type entryType)
    {
        return from type in GetCustomAssemblies(entryType).SelectMany(SafeGetTypes).ExceptNullItems()
            let allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            from property in allProperties
            let attribute = property.GetCustomAttributes<PropertyDependencyAttribute>(false).FirstOrDefault()
            where attribute != null
            let firstInvalidDependency = attribute.PropertyNames.FirstOrDefault(referencedProperty => !allProperties.Any(p => string.Equals(p?.Name, referencedProperty, StringComparison.Ordinal)))
            where firstInvalidDependency != null
            select type.FullName + "." + property.Name + " has invalid dependency: " + firstInvalidDependency;
    }

    /// <summary>
    /// Gets the custom assemblies (i.e. assemblies in the same folder or sub-folder) referenced by the assembly of the specified type.
    /// </summary>
    /// <param name="entryType">A type contained in the entry assembly.</param>
    /// <returns>The assembly that contains the entryType plus all custom assemblies that this assembly references.</returns>
    private static IEnumerable<Assembly> GetCustomAssemblies(Type entryType)
    {
        var entryAssembly = entryType.Assembly;

        var programFolder = Path.GetDirectoryName(entryAssembly.GetAssemblyDirectory().FullName)!;

        var referencedAssemblyNames = entryAssembly.GetReferencedAssemblies();

        var referencedAssemblies = referencedAssemblyNames
            .Select(SafeLoad)
            .ExceptNullItems()
            .Where(assembly => IsAssemblyInSubfolderOf(assembly, programFolder));

        return new[] { entryAssembly }.Concat(referencedAssemblies);
    }

    /// <summary>
    /// Determines whether the assembly is located in the same folder or a sub folder of the specified program folder.
    /// </summary>
    /// <param name="assembly">Name of the assembly.</param>
    /// <param name="programFolder">The program folder.</param>
    /// <returns>
    ///   <c>true</c> if the assembly is located in the same folder or a sub folder of the specified program folder; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsAssemblyInSubfolderOf(Assembly assembly, string programFolder)
    {
        var assemblyDirectory = Path.GetDirectoryName(assembly.GetAssemblyDirectory().FullName)!;

        return assemblyDirectory.StartsWith(programFolder, StringComparison.OrdinalIgnoreCase);
    }

    private static Assembly? SafeLoad(AssemblyName name)
    {
        try
        {
            return Assembly.Load(name);
        }
        catch (ReflectionTypeLoadException)
        {
        }

        return null;
    }


    private static IEnumerable<Type> SafeGetTypes(Assembly a)
    {
        try
        {
            return a.GetTypes();
        }
        catch (ReflectionTypeLoadException)
        {
        }

        return Type.EmptyTypes;
    }
}
