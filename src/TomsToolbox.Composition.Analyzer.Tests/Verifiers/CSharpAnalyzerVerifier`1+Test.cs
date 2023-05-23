using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;

public static partial class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    static IEnumerable<MetadataReference> SystemReferences(params string[] files)
    {
        return files.Select(file => MetadataReference.CreateFromFile(Path.Combine(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319", file)));
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Designed by Microsoft")]
    public class Test : CSharpAnalyzerTest<TAnalyzer, XUnitVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                var systemReferences = SystemReferences("mscorlib.dll", "System.Runtime.dll", "System.ComponentModel.Composition.dll");
                var localReferences = new[] { typeof(System.Composition.ExportAttribute) }.Select(type => MetadataReference.CreateFromFile(type.Assembly.Location));

                solution = solution.WithProjectMetadataReferences(projectId, systemReferences.Concat(localReferences));

                return solution;
            });
        }
    }
}

