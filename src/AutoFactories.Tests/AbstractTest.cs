using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Ninject.AutoFactories;
using Xunit.Abstractions;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Text;

namespace AutoFactories.Tests
{
    public abstract class AbstractTest
    {
        protected readonly ITestOutputHelper m_outputHelper;
        private static readonly MetadataReference[] s_metadataReferences;
        private static readonly VerifySettings s_verifySettings;
        private static readonly CSharpCompilationOptions s_cSharpCompilationOptions;

        static AbstractTest()
        {
            s_verifySettings = new VerifySettings();
            s_verifySettings.UseDirectory("Snapshots");
            s_cSharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable);
            s_metadataReferences = [
                MetadataReference.CreateFromFile(typeof(Options).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)];

            VerifySourceGenerators.Initialize();
            GeneratorDriverResultFilter.Initialize();
        }


        protected AbstractTest(ITestOutputHelper outputHelper)
        {
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
            string[]? verifySource = null,
            List<string>? notes = null,
            Action<IEnumerable<Diagnostic>>? assertAnalyuzerResult = null)
        {
            SyntaxTree[] syntaxTrees = [CSharpSyntaxTree.ParseText(source)];
            CSharpCompilation baseCompilation = CSharpCompilation.Create("UnitTest", syntaxTrees, s_metadataReferences, s_cSharpCompilationOptions);

            // Setup Source Generator 
            AutoFactoriesGenerator generator = new AutoFactoriesGenerator();
            AutoFactoriesGeneratorHoist incrementalGenerator = new AutoFactoriesGeneratorHoist(generator);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(incrementalGenerator);
            driver = driver.RunGeneratorsAndUpdateCompilation(baseCompilation, out Compilation finalCompilation, out ImmutableArray<Diagnostic> diagnostics);

            // Do compile check 
            bool hasError = false;
            StringBuilder builder = new StringBuilder()
                .AppendLine("Test case failed as the source code failed to compile");
            foreach(Diagnostic diagnostic in finalCompilation.GetDiagnostics())
            {
                switch (diagnostic.Severity)
                {
                    case DiagnosticSeverity.Info:
                    case DiagnosticSeverity.Warning:
                    case DiagnosticSeverity.Error:
                        hasError = true;
                        break;
                    default:
                        continue;
                }
                
                builder.AppendLine($"{diagnostic.Severity} {diagnostic.Id}: {diagnostic.GetMessage()}");
            }
            if (hasError)
            {
                Assert.Fail(builder.ToString());
            }

            // Assert Analyzer
            if (assertAnalyuzerResult != null)
            {
                AnalysisResult analysisResults = await finalCompilation.WithAnalyzers([new AutoFactoriesAnalyzer()])
                    .GetAnalysisResultAsync(CancellationToken.None);

                assertAnalyuzerResult(analysisResults.GetAllDiagnostics());
            }

            // Assert Source Trees
            if (verifySource != null)
            {
                GeneratorDriverRunResult runResults = driver.GetRunResult();
                GeneratorDriverResultFilter filter = new(runResults, verifySource.Contains)
                {
                    Notes = notes ?? new List<string>()
                };

                await Verifier.Verify(filter, s_verifySettings);
            }
        }
    }
}