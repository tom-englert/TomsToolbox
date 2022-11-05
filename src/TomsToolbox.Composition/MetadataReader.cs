namespace TomsToolbox.Composition;

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

        var isMef1 = anyExportAttributeType.ExportAttributeType?.Namespace == "System.ComponentModel.Composition";

        result.Add(new ExportInfo(type, isMef1, exportAttributes.Select(item => item.Attribute)));
    }
}
