using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;

namespace AutoFactories.Tests
{
    public class UnitTestCompiler
    {
        private readonly ImmutableArray<MetadataReference> m_references;
        private readonly CSharpCompilationOptions m_cSharpCompileOptions;

        public ImmutableArray<IIncrementalGenerator> Generators { get; }
        public ImmutableArray<DiagnosticAnalyzer> Analyzers { get; }
        public ImmutableArray<AdditionalText> AdditionalTexts { get; }

        public UnitTestCompiler(
            IEnumerable<IIncrementalGenerator>? generators = null,
            IEnumerable<DiagnosticAnalyzer>? analyzers = null,
            IEnumerable<AdditionalText>? additionalTexts = null)
        {
            generators ??= Array.Empty<IIncrementalGenerator>();
            analyzers ??= Array.Empty<DiagnosticAnalyzer>();
            additionalTexts ??= Array.Empty<AdditionalText>();

            m_cSharpCompileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable);

            Generators = generators.ToImmutableArray();
            Analyzers = analyzers.ToImmutableArray();
            AdditionalTexts = additionalTexts.ToImmutableArray();

            m_references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !string.IsNullOrWhiteSpace(a.Location))
                .Where(a => !a.IsDynamic)
                .Select<Assembly, MetadataReference>(a => MetadataReference.CreateFromFile(a.Location))
                .ToImmutableArray();
        }

        public async Task<CompileResult> CompileAsync(SyntaxTree[] syntaxTrees)
        {
            Compilation compilation = CSharpCompilation.Create("UnitTests", syntaxTrees, m_references, m_cSharpCompileOptions);


            List<Diagnostic> diagnostics = new List<Diagnostic>();

            // Run one at a time
            foreach (IIncrementalGenerator generator in Generators)
            {
                _ = CSharpGeneratorDriver.Create(generator)
                 .AddAdditionalTexts(AdditionalTexts)
                 .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out ImmutableArray<Diagnostic> generatedDiagnostics);
                diagnostics.AddRange(generatedDiagnostics);
            }

            if (Analyzers.Length > 0)
            {
                CompilationWithAnalyzers analyzers = compilation.WithAnalyzers(Analyzers);
                diagnostics.AddRange(await analyzers.GetAllDiagnosticsAsync());
            }
            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTrees[0]);

            return new CompileResult(compilation, semanticModel, [..diagnostics]);
        }
    }
}
