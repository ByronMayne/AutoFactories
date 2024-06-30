using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Ninject.AutoFactories
{
    /// <summary>
    /// A unit test base class for writing tests for Source Generators.
    /// </summary>
    public abstract class SourceGeneratorTestBase
    {
        protected ITestOutputHelper m_outputHelper;

        protected SourceGeneratorTestBase(ITestOutputHelper outputHelper)
        {
            m_outputHelper = outputHelper;
        }

        /// <summary>
        /// Writes a line of text that will show up in the output of the unit test
        /// </summary>
        /// <param name="text">The text to write</param>
        protected void WriteLine(string text)
            => m_outputHelper.WriteLine(text);

        /// <summary>
        /// Verifies that the source sent in here generates into the expected types.
        /// </summary>
        /// <param name="source">The source to try to run the source generator on</param>
        /// <returns>A task to await on</returns>
        protected static Task Compose(
            string? source = "", 
            Predicate<string>[]? filters = null)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "AutoFactoriesTests",
                syntaxTrees: new[] { syntaxTree });

            // The 'hoist' is the SGF libraries wrapper around source generators
            AutoFactorySourceGeneratorHoist generator = new AutoFactorySourceGeneratorHoist();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            VerifySettings settings = new VerifySettings();
            settings.UseDirectory("Snapshots");

            GeneratorDriverResultFilter filter = new GeneratorDriverResultFilter(driver.GetRunResult(), filters);

            return Verifier.Verify(filter, settings);
        }

        protected static class Filters
        {
            public static Predicate<string> Only(string fileName)
                => f => string.Equals(fileName, f);
        }
    }
}
