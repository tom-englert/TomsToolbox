namespace TomsToolbox.Composition
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Export information for a specific type.
    /// </summary>
    public class ExportInfo
    {
        private const string SharedAttributeName = "SharedAttribute";
        private const string PartCreationPolicyAttributeName = "PartCreationPolicyAttribute";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportInfo"/> class.
        /// </summary>
        public ExportInfo()
        {
        }

        internal ExportInfo(Type type, bool isMef1, IEnumerable<CustomAttributeData> exportAttributes)
        {
            Type = type;
            IsShared = isMef1;
            Metadata = exportAttributes
                .Select(ReadMetadata)
                .ToArray();

            if (isMef1)
                EvaluatePartCreationPolicy(type);
            else
                EvaluateSharedAttribute(type);
        }

        /// <summary>
        /// Gets or sets the type this export information relates to.
        /// </summary>
        public Type? Type { get; set; }

        /// <summary>
        /// Gets or sets the metadata of each export.
        /// </summary>
        public IDictionary<string, object?>[]? Metadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is marked as being constrained to sharing within the specified boundary.
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Gets or sets the boundary outside which the part marked by this attribute is inaccessible.
        /// </summary>
        public string? SharingBoundary { get; set; }

        private void EvaluateSharedAttribute(Type type)
        {
            var sharedAttribute = type.GetCustomAttributesData()
                .FirstOrDefault(attr => attr.AttributeType.Name == SharedAttributeName);

            if (sharedAttribute != null)
            {
                IsShared = true;
                SharingBoundary = sharedAttribute.ConstructorArguments.Select(arg => arg.Value as string).FirstOrDefault();
            }
        }

        private void EvaluatePartCreationPolicy(Type type)
        {
            var partCreationPolicyAttribute = type.GetCustomAttributesData()
                .FirstOrDefault(attr => attr.AttributeType.Name == PartCreationPolicyAttributeName);

            if (partCreationPolicyAttribute != null)
            {
                var value = partCreationPolicyAttribute.ConstructorArguments.Select(arg => (int)arg.Value).FirstOrDefault();
                IsShared = value != 2;
            }
        }

        private IDictionary<string, object?> ReadMetadata(CustomAttributeData exportAttribute)
        {
            var metadata = new Dictionary<string, object?>();

            GetConstructorParameters(exportAttribute, metadata);
            GetNamedArguments(exportAttribute, metadata);

            return metadata;
        }

        private static void GetConstructorParameters(CustomAttributeData exportAttribute, IDictionary<string, object?> metadata)
        {
            var instance = exportAttribute.Constructor.Invoke(exportAttribute.ConstructorArguments.Select(ConvertValue).ToArray());
            var properties = exportAttribute.AttributeType.GetProperties();

            foreach (var propertyInfo in properties.OrderBy(item => item.Name))
            {
                var value = propertyInfo.GetValue(instance, null);

                metadata[propertyInfo.Name] = value;
            }
        }

        private static void GetNamedArguments(CustomAttributeData exportAttribute, IDictionary<string, object?> metadata)
        {
            var namedArguments = exportAttribute.NamedArguments;
            if (namedArguments == null)
                return;

            foreach (var namedArgument in namedArguments.OrderBy(item => item.MemberName))
            {
                metadata[namedArgument.MemberName] = ConvertValue(namedArgument.TypedValue)!;
            }
        }

        private static object? ConvertValue(CustomAttributeTypedArgument argument)
        {
            return ConvertValue(argument.ArgumentType, argument.Value);
        }

        private static object? ConvertValue(Type argumentType, object? value)
        {
            if (value == null)
                return null;

            var valueType = value.GetType();

            if (argumentType.IsAssignableFrom(valueType))
                return value;

            if (argumentType.IsArray)
            {
                var elementType = argumentType.GetElementType();
                if (elementType != null)
                {
                    var values = (IEnumerable<CustomAttributeTypedArgument>)value;
                    var arrayList = ArrayList.Adapter(values.Select(ConvertValue).ToList());

                    return arrayList.ToArray(elementType);
                }
            }

            if (argumentType.IsEnum)
            {
                return Enum.Parse(argumentType, value.ToString());
            }

            throw new InvalidOperationException($"Argument type conversion from {valueType} to {argumentType} is not supported.");
        }
    }

    public static partial class ExtensionMethods
    {
        private const string ContractName = "ContractName";
        private const string ContractType = "ContractType";

        /// <summary>
        /// Reads the well known ContractName property from the <paramref name="metadata"/>.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The contract name</returns>
        public static string? GetContractName(this IMetadata metadata)
        {
            return metadata.GetValueOrDefault<string>(ContractName);
        }

        /// <summary>
        /// Reads the well known ContractName property from the <paramref name="metadata"/>.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The contract name</returns>
        public static string? GetContractName(this IDictionary<string, object?> metadata)
        {
            return metadata.TryGetValue(ContractName, out var value) ? value as string : null;
        }

        /// <summary>
        /// Reads the well known ContractType property from the <paramref name="metadata"/>.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The contract type</returns>
        public static Type? GetContractType(this IDictionary<string, object?> metadata)
        {
            return metadata.GetValueOrDefault(ContractType) as Type;
        }

        /// <summary>
        /// Checks whether the <paramref name="contractName"/> matches the value stored in the <paramref name="metadata"/>.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="contractName">Name of the contract.</param>
        /// <returns><c>true</c> if there is a match.</returns>
        public static bool ContractNameMatches(this IMetadata metadata, string? contractName)
        {
            var value = metadata.GetContractName();

            return string.IsNullOrEmpty(value) == string.IsNullOrEmpty(contractName) || string.Equals(value, contractName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the contract type for the specified <paramref name="elementType"/>.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="elementType">Type of the element.</param>
        /// <returns>The contract type stored in the <paramref name="metadata"/>; returns <c>null</c> if the contract type in the metadata matches the element type.</returns>
        public static Type? GetContractTypeFor(this IDictionary<string, object?> metadata, Type elementType)
        {
            var type = metadata.GetContractType();
            
            return type == elementType ? null : type;
        }

        /// <summary>
        /// Gets the default metadata for the specified service.
        /// </summary>
        /// <param name="serviceAndImplementationType">Type of the service and implementation.</param>
        /// <param name="contractName">Optional name of the contract.</param>
        /// <returns>The metadata dictionary</returns>
        public static IDictionary<string, object?> GetDefaultMetadata(this Type serviceAndImplementationType, string? contractName = null)
        {
            return GetDefaultMetadata(serviceAndImplementationType, null, contractName);

        }

        /// <summary>
        /// Gets the default metadata for the specified service.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="serviceType">Type of the service, or <c>null</c> if service type and implementation type are the same.</param>
        /// <param name="contractName">Optional name of the contract.</param>
        /// <returns>The metadata dictionary</returns>
        public static IDictionary<string, object?> GetDefaultMetadata(this Type implementationType, Type? serviceType = null, string? contractName = null)
        {
            return new Dictionary<string, object?>
            {
                [ContractType] = serviceType != implementationType ? serviceType : null,
                [ContractName] = contractName
            };
        }
    }
}