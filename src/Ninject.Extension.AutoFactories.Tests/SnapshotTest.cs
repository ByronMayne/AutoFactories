using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Abstractions;

namespace Ninject.AutoFactories
{
    /// <summary>
    /// A unit test base class for writing tests for Source Generators.
    /// </summary>
    public abstract class SnapshotTest
    {
        protected ITestOutputHelper m_outputHelper;
        private readonly List<string> m_testSubjects;

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
        protected Task Compose(
            string? source = "")
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "AutoFactoriesTests",
                syntaxTrees: new[] { syntaxTree });

            // The 'hoist' is the SGF libraries wrapper around source generators
            AutoFactorySourceGeneratorHoist generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            VerifySettings settings = new();
            settings.UseDirectory("Snapshots");


            GeneratorDriverResultFilter filter = new(driver.GetRunResult(), m_testSubjects.Contains);

            return Verifier.Verify(filter, settings);
        }
    }
}
