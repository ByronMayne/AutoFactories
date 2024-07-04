using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Ninject.AutoFactories
{
    /// <summary>
    /// A unit test base class for writing tests for Source Generators.
    /// </summary>
    public abstract class SnapshotTest
    {
        protected ITestOutputHelper m_outputHelper;
        private readonly List<string> m_testSubjects;

        static SnapshotTest()
        {
            VerifySourceGenerators.Initialize();
            VerifierSettings.RegisterFileConverter<GeneratorDriverResultFilter>(Convert);
        }

        protected SnapshotTest(ITestOutputHelper outputHelper)
        {
            m_testSubjects = [];
            m_outputHelper = outputHelper;
        }

        /// <summary>
        /// Adds the a file to be tested by the snapshot tests. This is the `HintName` of the generated class
        /// </summary>
        protected void AddTestSubject(string fileName)
        {
            m_testSubjects.Add(fileName);
        }

        /// <summary>
        /// Writes a line of text that will show up in the output of the unit test
        /// </summary>
        /// <param name="text">The text to write</param>
        protected void WriteLine(string text)
        {
            m_outputHelper.WriteLine(text);
        }

        /// <summary>
        /// Verifies that the source sent in here generates into the expected types.
        /// </summary>
        /// <param name="source">The source to try to run the source generator on</param>
        /// <returns>A task to await on</returns>
        protected async Task Compose(
            string? source = "")
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            IEnumerable<PortableExecutableReference> references = new[]
            {
                 MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "AutoFactoriesTests",
                syntaxTrees: new[] { syntaxTree },
                references: references);

            // The 'hoist' is the SGF libraries wrapper around source generators
            AutoFactorySourceGeneratorHoist generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

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

            GeneratorDriverResultFilter filter = new(runResults, m_testSubjects.Contains);

            await Verifier.Verify(filter, settings);
        }


        private static ConversionResult Convert(GeneratorDriverResultFilter target, IReadOnlyDictionary<string, object> context)
        {
            var exceptions = new List<Exception>();
            var targets = new List<Target>();
            foreach (var result in target.Result.Results)
            {
                if (result.Exception != null)
                {
                    exceptions.Add(result.Exception);
                }

                var collection = result.GeneratedSources
                    .Where(x => target.Include(x.HintName))
                    .OrderBy(x => x.HintName)
                    .Select(SourceToTarget);

                if (!collection.Any())
                {
                    Assert.Fail($"No tests subjects matched any of the patterns. The following subjects were found\n{string.Join("\n - ", result.GeneratedSources.Select(s => s.HintName))}");
                }

                targets.AddRange(collection);
            }

            if (exceptions.Count == 1)
            {
                throw exceptions.First();
            }

            if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }

            if (target.Result.Diagnostics.Any())
            {
                var info = new
                {
                    target.Result.Diagnostics
                };
                return new(info, targets);
            }

            return new(null, targets);
        }


        private static Target SourceToTarget(GeneratedSourceResult source)
        {
            var data = $"""
            //HintName: {source.HintName}
            {source.SourceText}
            """;
            return new("cs", data, Path.GetFileNameWithoutExtension(source.HintName));
        }
    }
}
