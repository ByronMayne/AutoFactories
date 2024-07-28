using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Ninject.AutoFactories;
using Xunit.Abstractions;
using System.Collections.Immutable;

namespace AutoFactories.Tests
{
    public abstract class AbstractTest
    {
        protected readonly ITestOutputHelper m_outputHelper;
        private readonly List<string> m_testSubjects;

        static AbstractTest()
        {
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
            MetadataReference[] metadataReferences = [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)];


            CSharpCompilation compilation = CSharpCompilation.Create("UnitTest", syntaxTrees, metadataReferences);

            AutoFactorySourceGeneratorHoist generatorHost = new AutoFactorySourceGeneratorHoist();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generatorHost);
            driver = driver.RunGenerators(compilation);

            VerifySettings settings = new();
            settings.UseDirectory("Snapshots");

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
            ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics();

            GeneratorDriverResultFilter filter = new(runResults, m_testSubjects.Contains);

            if (verifyOutput) await Verifier.Verify(filter, settings);
            if (assertDiagnostics is not null) assertDiagnostics(diagnostics);
        }
    }
}