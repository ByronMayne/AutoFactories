using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Ninject.AutoFactories;
using Xunit.Abstractions;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoFactories.Tests
{
    public abstract class AbstractTest
    {
        protected readonly ITestOutputHelper m_outputHelper;
        private readonly List<string> m_testSubjects;
        private static readonly VerifySettings s_verifySettings;

        static AbstractTest()
        {
            s_verifySettings = new VerifySettings();
            s_verifySettings.UseDirectory("Snapshots");

            VerifySourceGenerators.Initialize();
            GeneratorDriverResultFilter.Initialize();
        }

        protected AbstractTest(ITestOutputHelper outputHelper)
        {
            m_testSubjects = [];
      
            m_outputHelper = outputHelper;
        }

        /// <summary>
        /// Writes a line of text that will show up in the output of the unit test
        /// </summary>
        /// <param name="text">The text to write</param>
        protected void WriteLine(string text)
        {
            m_outputHelper.WriteLine(text);
        }


        protected async Task Compose(
            string source,
            bool verifyOutput = false,
            Action<IEnumerable<Diagnostic>>? assertDiagnostics = null)
        {
            SyntaxTree[] syntaxTrees = [CSharpSyntaxTree.ParseText(source)];
            MetadataReference[] metadataReferences = [

                MetadataReference.CreateFromFile(typeof(Options).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)];


            CSharpCompilationOptions options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithAllowUnsafe(true)
                .WithDeterministic(true);

            ImmutableArray<DiagnosticAnalyzer> analyzers = [];

            CSharpCompilation compilation = CSharpCompilation.Create("UnitTest", syntaxTrees, metadataReferences, options);

            // Source Generator 
            AutoFactoriesGenerator generator = new AutoFactoriesGenerator();
            AutoFactoriesGeneratorHoist incrementalGenerator = new AutoFactoriesGeneratorHoist(generator);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(incrementalGenerator)
                .RunGeneratorsAndUpdateCompilation(compilation, out Compilation postCompilation, out ImmutableArray<Diagnostic> _);

            // Code Analyzer 
            AnalysisResult analysisResults = await postCompilation.WithAnalyzers([new AutoFactoriesAnalyzer()])
                .GetAnalysisResultAsync(CancellationToken.None);

            ImmutableArray<Diagnostic> diagnostics = analysisResults.GetAllDiagnostics();
            GeneratorDriverRunResult runResults = driver.GetRunResult();

#if DEBUG
            foreach (GeneratorRunResult result in runResults.Results)
            {
                foreach (GeneratedSourceResult s in result.GeneratedSources)
                {
                    WriteLine(s.HintName);
                }
            }
#endif
            GeneratorDriverResultFilter filter = new(runResults, m_testSubjects.Contains);

            if (verifyOutput) await Verifier.Verify(filter, s_verifySettings);
            if (assertDiagnostics is not null) assertDiagnostics(diagnostics);
        }
    }
}