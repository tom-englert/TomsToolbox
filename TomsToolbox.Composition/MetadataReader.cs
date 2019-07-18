namespace TomsToolbox.Composition
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

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
        /// <returns></returns>
        public static IList<ExportInfo> Read([NotNull] Assembly assembly)
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
                .Where(attr => attr.AttributeType.GetSelfAndBaseTypes().Any(t => t.Name == ExportAttributeName))
                .ToList();

            if (!exportAttributes.Any())
                return;

            result.Add(new ExportInfo(type, exportAttributes));
        }
    }

    /// <summary>
    /// Export information for a specific type.
    /// </summary>
    public class ExportInfo
    {
        private const string SharedAttributeName = "SharedAttribute";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportInfo"/> class.
        /// </summary>
        public ExportInfo()
        {
        }

        internal ExportInfo(Type type, IList<CustomAttributeData> exportAttributes)
        {
            Type = type;
            Metadata = exportAttributes
                .Select(ReadMetadata)
                .ToArray();
            
            EvaluateSharedAttribute(type);
        }

        /// <summary>
        /// Gets or sets the type this export information relates to.
        /// </summary>
        [CanBeNull]
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets the metadata of each export.
        /// </summary>
        [CanBeNull]
        public IMetadata[] Metadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is marked as being constrained to sharing within the specified boundary.
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// Gets or sets the boundary outside which the part marked by this attribute is inaccessible.
        /// </summary>
        [CanBeNull]
        public string SharingBoundary { get; set; }

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

        private IMetadata ReadMetadata(CustomAttributeData exportAttribute)
        {
            var metadata = new Dictionary<string, object>();

            GetConstructorParameters(exportAttribute, metadata);
            GetNamedArguments(exportAttribute, metadata);

            return new MetadataAdapter(metadata);
        }

        private static void GetConstructorParameters(CustomAttributeData exportAttribute, IDictionary<string, object> metadata)
        {
            var instance = exportAttribute.Constructor.Invoke(exportAttribute.ConstructorArguments.Select(ConvertValue).ToArray());
            var properties = exportAttribute.AttributeType.GetProperties();

            foreach (var propertyInfo in properties)
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

            foreach (var namedArgument in namedArguments)
            {
                metadata[namedArgument.MemberName] = ConvertValue(namedArgument.TypedValue);
            }
        }

        [CanBeNull]
        private static object ConvertValue(CustomAttributeTypedArgument argument)
        {
            return ConvertValue(argument.ArgumentType, argument.Value);
        }

        [CanBeNull]
        private static object ConvertValue(Type argumentType, [CanBeNull] object value)
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
                    var values = (IEnumerable<CustomAttributeTypedArgument>) value;
                    var arrayList = ArrayList.Adapter(values.Select(ConvertValue).ToList());

                    return arrayList.ToArray(elementType);
                }
            }

            throw new InvalidOperationException($"Argument type conversion from {valueType} to {argumentType} is not supported.");
        }
    }
}
