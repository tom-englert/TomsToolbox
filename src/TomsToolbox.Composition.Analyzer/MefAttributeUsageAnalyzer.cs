using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Immutable;

namespace TomsToolbox.Composition.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MefAttributeUsageAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => AllDiagnostics;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeClassNode, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeAttributeNode, SyntaxKind.Attribute);
    }

    private static void AnalyzeAttributeNode(SyntaxNodeAnalysisContext context)
    {
        var cancellationToken = context.CancellationToken;
        var attribute = (AttributeSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol is not IMethodSymbol attributeSymbol)
            return;

        var attributeType = attributeSymbol.ContainingType;
        var namespaceName = attributeType.ContainingNamespace.ToDisplayString(FullNameDisplayFormat);

        if (attributeType.IsOneOfTheseCompositionTypes("ImportAttribute", "ImportManyAttribute"))
        {
            context.ReportDiagnostic(Diagnostic.Create(AvoidImportAttributesRule, attribute.Name.GetLocation()));
        }
    }

    private static void AnalyzeClassNode(SyntaxNodeAnalysisContext context)
    {
        var cancellationToken = context.CancellationToken;
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        if (context.ContainingSymbol is not INamedTypeSymbol typeSymbol)
            return;

        ICollection<INamedTypeSymbol> GetAttributeTypes(AttributeSyntax attributeSyntax)
        {
            if (context.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol methodSymbol)
                return Array.Empty<INamedTypeSymbol>();

            return methodSymbol.ContainingType
                .EnumerateSelfAndBaseTypes()
                .Where(type => type.IsCompositionType())
                .ToArray();
        }

        bool IsTypeExpectedToBeNonShared(out string info)
        {
            info = string.Empty;

            var baseTypeNames = typeSymbol.EnumerateBaseTypes()
                .Select(baseType => baseType.ToDisplayString(FullNameDisplayFormat))
                .ToArray();

            info = $"{typeSymbol.ToDisplayString(FullNameDisplayFormat)} : {string.Join(" : ", baseTypeNames)}";

            return baseTypeNames
                .Any(baseType => baseType is "System.Windows.UIElement" or "System.Web.Http.ApiController");
        }

        var attributes = classDeclaration.AttributeLists.SelectMany(list => list.Attributes)
            .Select(attribute => new { Attribute = attribute, Types = GetAttributeTypes(attribute) })
            .Where(attribute => attribute.Types.Any())
            .ToArray();

        var exportAttributeType = attributes.SelectMany(attr => attr.Types).FirstOrDefault(attr => attr.ToDisplayString(NameDisplayFormat) == "ExportAttribute");

        if (exportAttributeType == null)
            return;

        var isMef1 = exportAttributeType.ContainingNamespace.ToDisplayString(FullNameDisplayFormat) is "System.ComponentModel.Composition";

        if (isMef1)
        {
            var creationPolicyAttribute = attributes.FirstOrDefault(attr => attr.Types.Any(type => type.ToDisplayString(NameDisplayFormat) == "PartCreationPolicyAttribute"));

            if (creationPolicyAttribute is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(NoCreationPolicyRuleMef1, classDeclaration.Identifier.GetLocation()));
            }

            if (IsTypeExpectedToBeNonShared(out var info))
            {
                var param = creationPolicyAttribute?.Attribute.ArgumentList?.Arguments.FirstOrDefault();
                var configuredAsShared = param?.ToString() != "CreationPolicy.NonShared";

                if (configuredAsShared)
                    context.ReportDiagnostic(Diagnostic.Create(SuspiciousPolicyRule, classDeclaration.Identifier.GetLocation(), info));
            }
        }
        else
        {
            var creationPolicyAttributes = attributes
                .SelectMany(attr => attr.Types.Select(type => type.ToDisplayString(NameDisplayFormat)))
                .Where(type => type is "SharedAttribute" or "NonSharedAttribute")
                .ToArray();

            switch (creationPolicyAttributes.Length)
            {
                case 0:
                    context.ReportDiagnostic(Diagnostic.Create(NoCreationPolicyRuleMef2, classDeclaration.Identifier.GetLocation()));
                    break;

                case 1:
                    break;

                default:
                    context.ReportDiagnostic(Diagnostic.Create(MultipleCreationPolicyRule, classDeclaration.Identifier.GetLocation()));
                    return;
            }

            if (IsTypeExpectedToBeNonShared(out var info))
            {
                var configuredAsShared = creationPolicyAttributes.FirstOrDefault() is "SharedAttribute";

                if (configuredAsShared)
                    context.ReportDiagnostic(Diagnostic.Create(SuspiciousPolicyRule, classDeclaration.Identifier.GetLocation(), info));
            }
        }

        if (typeSymbol.HasNonDefaultConstructors(out var constructors))
        {
            var importingConstructors = constructors
                .Where(ctor => ctor.GetAttributes().Any(attr => attr.AttributeClass?.IsOneOfTheseCompositionTypes("ImportingConstructorAttribute") == true))
                .ToArray();

            switch (importingConstructors.Length)
            {
                case 0:
                    context.ReportDiagnostic(Diagnostic.Create(NoImportingConstructorRule, classDeclaration.Identifier.GetLocation()));
                    if (!constructors.Any(ctor => ctor.DeclaredAccessibility == Accessibility.Public))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(NoPublicConstructorRule, classDeclaration.Identifier.GetLocation()));
                    }
                    break;

                case 1:
                    if (importingConstructors[0].DeclaredAccessibility != Accessibility.Public)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(NoPublicConstructorRule, classDeclaration.Identifier.GetLocation()));
                    }
                    break;

                default:
                    context.ReportDiagnostic(Diagnostic.Create(MultipleImportingConstructorsRule, classDeclaration.Identifier.GetLocation()));
                    break;
            }
        }
    }
}
