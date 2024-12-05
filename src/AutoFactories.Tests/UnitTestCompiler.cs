using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using System.Collections.Immutable;

namespace AutoFactories.Tests
{
    internal class UnitTestCompiler
    {
        private ImmutableArray<MetadataReference> m_references;
        private readonly CSharpCompilationOptions m_csharpCompileOptions;
        private readonly AutoFactoriesGeneratorHoist m_generatorHost;

        public UnitTestCompiler()
        {
            m_csharpCompileOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable);

            AutoFactoriesGenerator generator = new AutoFactoriesGenerator();
            generator.ExceptionHandler += OnGeneratorException;

            m_generatorHost = new AutoFactoriesGeneratorHoist(generator);

            m_references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !string.IsNullOrWhiteSpace(a.Location))
                .Where(a => !a.IsDynamic)
                .Select<Assembly, MetadataReference>(a => MetadataReference.CreateFromFile(a.Location))
                .ToImmutableArray();
        }


        public CompileResult Compile(string[] source, AdditionalText[]? additionalTexts = null)
        {
            additionalTexts ??= Array.Empty<AdditionalText>();

            SyntaxTree[] syntaxTrees = source
                .Select(s => CSharpSyntaxTree.ParseText(s))
                .ToArray();

            CSharpCompilation compilation = CSharpCompilation.Create("UnitTests", syntaxTrees, m_references, m_csharpCompileOptions);
            GeneratorDriver driver = CSharpGeneratorDriver.Create(m_generatorHost)
                .AddAdditionalTexts(additionalTexts.ToImmutableArray())
                .RunGeneratorsAndUpdateCompilation(compilation, out Compilation generatorCompilation, out ImmutableArray<Diagnostic> _);

            SemanticModel semanticModel = generatorCompilation.GetSemanticModel(syntaxTrees[0]);
            ImmutableArray<Diagnostic> diagnostics = generatorCompilation.GetDiagnostics();

            return new CompileResult(syntaxTrees[0], generatorCompilation, semanticModel, diagnostics);
        }


        /// <summary>
        /// Invoked when the generator throws an exception
        /// </summary>
        /// <param name="exception"></param>
        private void OnGeneratorException(Exception exception)
        {
            Assert.Fail($"An unhandled exception was thrown while generating.\n{exception}");
        }
    }
}
