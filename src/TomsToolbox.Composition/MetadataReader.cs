namespace TomsToolbox.Composition
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using TomsToolbox.Essentials;

    /// <summary>
    /// Metadata reader the for MEF 1.0 (System.ComponentModel.Composition) or MEF 2.0 (System.Composition.AttributedModel) attributes.
    /// </summary>
    public static class MetadataReader
    {
        private const string ExportAttributeName = "ExportAttribute";

        /// <summary>
        /// Reads the export info and metadata from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The export info for each type with an export attribute in the assembly.</returns>
        public static IList<ExportInfo> Read(Assembly assembly)
        {
            var result = new List<ExportInfo>();

            ReadTypes(assembly.DefinedTypes, result);

            return result;
        }

        private static void ReadTypes(IEnumerable<TypeInfo> typeInfos, IList<ExportInfo> result)
        {
            foreach (var typeInfo in typeInfos)
            {
                ReadType(typeInfo, result);
                ReadTypes(typeInfo.DeclaredNestedTypes, result);
            }
        }

        private static void ReadType(TypeInfo typeInfo, IList<ExportInfo> result)
        {
            var type = typeInfo.AsType();

            var exportAttributes = type.GetCustomAttributesData()
                .Select(attr => new { Attribute = attr, ExportAttributeType = attr.AttributeType.GetSelfAndBaseTypes().FirstOrDefault(t => t.Name == ExportAttributeName) })
                .Where(item => item.ExportAttributeType != null)
                .ToList();

            if (!exportAttributes.Any())
                return;

            var anyExportAttributeType = exportAttributes.First();

            var isMef1 = anyExportAttributeType.ExportAttributeType.Namespace == "System.ComponentModel.Composition";

            result.Add(new ExportInfo(type, isMef1, exportAttributes.Select(item => item.Attribute)));
        }
    }

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
        public IDictionary<string, object>[]? Metadata { get; set; }

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

        private IDictionary<string, object> ReadMetadata(CustomAttributeData exportAttribute)
        {
            var metadata = new Dictionary<string, object>();

            GetConstructorParameters(exportAttribute, metadata);
            GetNamedArguments(exportAttribute, metadata);

            return metadata;
        }

        private static void GetConstructorParameters(CustomAttributeData exportAttribute, IDictionary<string, object> metadata)
        {
            var instance = exportAttribute.Constructor.Invoke(exportAttribute.ConstructorArguments.Select(ConvertValue).ToArray());
            var properties = exportAttribute.AttributeType.GetProperties();

            foreach (var propertyInfo in properties.OrderBy(item => item.Name))
            {
                var value = propertyInfo.GetValue(instance, null);

                metadata[propertyInfo.Name] = value;
            }
        }

        private static void GetNamedArguments(CustomAttributeData exportAttribute, IDictionary<string, object> metadata)
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
}
