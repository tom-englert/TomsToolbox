global using static TomsToolbox.Composition.Analyzer.ExtensionMethods;

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace TomsToolbox.Composition.Analyzer;

public static class ExtensionMethods
{
    private const string Category = "DependencyInjection";

    public static readonly SymbolDisplayFormat FullNameDisplayFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static readonly SymbolDisplayFormat NameDisplayFormat = new(SymbolDisplayGlobalNamespaceStyle.Omitted,
        SymbolDisplayTypeQualificationStyle.NameOnly,
        SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static readonly DiagnosticDescriptor NoCreationPolicyRuleMef1 = new("MEF001",
        "Creation policy should be explicit",
        "Creation policy should be annotated explicitly. Consider adding a PartCreationPolicy attribute.",
        Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor AvoidImportAttributesRule = new("MEF002",
        "Avoid Import attributes",
        "Import attributes are not supported by all DI containers, consider to replace it with code.",
        Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor SuspiciousPolicyRule = new("MEF003",
        "Suspicious creation policy",
        "One of the base classes indicates that the creation policy should be non-shared: {0}",
        Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoCreationPolicyRuleMef2 = new("MEF004",
        "Creation policy should be explicit",
        "Creation policy should be annotated explicitly. Consider adding a Shared or NonShared attribute.",
        Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MultipleCreationPolicyRule = new("MEF005",
        "Creation policy ambiguous",
        "Creation policy is ambiguous, both Shared and NonShared attribute us used on the same type.",
        Category,
        DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoImportingConstructorRule = new("MEF006",
        "No importing constructor",
        "Exported classes should have one constructor marked with the ImportingConstructor attribute.",
        Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MultipleImportingConstructorsRule = new("MEF007",
        "Multiple importing constructors",
        "Exported classes should have only one constructor marked with the ImportingConstructor attribute.",
        Category,
        DiagnosticSeverity.Error, isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoPublicConstructorRule = new("MEF008",
        "No public constructor",
        "Exported classes should have at least one public constructor; if one is marked with the ImportingConstructor attribute, it must be public.",
        Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public static readonly ImmutableArray<DiagnosticDescriptor> AllDiagnostics = ImmutableArray.Create(
        NoCreationPolicyRuleMef1,
        AvoidImportAttributesRule,
        SuspiciousPolicyRule,
        NoCreationPolicyRuleMef2,
        MultipleCreationPolicyRule,
        NoImportingConstructorRule,
        MultipleImportingConstructorsRule,
        NoPublicConstructorRule
    );


    /*
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public static IEnumerable<T> Tap<T>(this IEnumerable<T> items, Action<IEnumerable<T>> action)
    {
        action(items);

        return items;
    }
    */

    public static IEnumerable<INamedTypeSymbol> EnumerateBaseTypes(this INamedTypeSymbol? typeSymbol)
    {
        while (true)
        {
            typeSymbol = typeSymbol?.BaseType;

            if (typeSymbol is null)
                yield break;

            yield return typeSymbol;
        }
    }

    public static IEnumerable<INamedTypeSymbol> EnumerateSelfAndBaseTypes(this INamedTypeSymbol? typeSymbol)
    {
        while (true)
        {
            if (typeSymbol is null)
                yield break;

            yield return typeSymbol;

            typeSymbol = typeSymbol.BaseType;
        }
    }

    public static bool IsCompositionType(this INamedTypeSymbol type)
    {
        return type.ContainingNamespace.ToDisplayString(FullNameDisplayFormat) is "System.ComponentModel.Composition"
            or "System.Composition";
    }

    public static bool IsMef1CompositionType(this INamedTypeSymbol type)
    {
        return type.ContainingNamespace.ToDisplayString(FullNameDisplayFormat) is "System.ComponentModel.Composition";
    }

    public static bool IsOneOfTheseCompositionTypes(this INamedTypeSymbol type, params string[] names)
    {
        var typeName = type.ToDisplayString(NameDisplayFormat);

        return type.ContainingNamespace.ToDisplayString(FullNameDisplayFormat) is "System.ComponentModel.Composition"
                   or "System.Composition"
               && names.Any(name => name == typeName);
    }

    public static bool HasNonDefaultConstructors(this INamedTypeSymbol typeSymbol, out ImmutableArray<IMethodSymbol> constructors)
    {
        constructors = typeSymbol.Constructors;

        return constructors.Length switch
        {
            0 => false,
            1 => constructors[0] is not { DeclaredAccessibility: Accessibility.Public, Parameters.Length: 0 },
            _ => true
        };
    }
}
