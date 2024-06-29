using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.AutoFactories
{
    /// <summary>
    /// A unit test base class for writing tests for Source Generators.
    /// </summary>
    internal abstract class SourceGeneratorTestBase
    {
        /// <summary>
        /// Verifies that the source sent in here generates into the expected types.
        /// </summary>
        /// <param name="source">The source to try to run the source generator on</param>
        /// <returns>A task to await on</returns>
        protected static Task Verify(string source)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "AutoFactoriesTests",
                syntaxTrees: new[] { syntaxTree });

            // The 'hoist' is the SGF libraries wrapper around source generators
            AutoFactorySourceGeneratorHoist generator = new AutoFactorySourceGeneratorHoist();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            return Verifier.Verify(driver);
        }
    }
}
