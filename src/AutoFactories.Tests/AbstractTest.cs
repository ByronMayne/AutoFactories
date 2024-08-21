using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Ninject.AutoFactories;
using Xunit.Abstractions;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Text;
using System.Reflection;

namespace AutoFactories.Tests
{
    public abstract class AbstractTest
    {
        private readonly List<ViewModule> m_viewModules;
        protected readonly ITestOutputHelper m_outputHelper;
        private static readonly VerifySettings s_verifySettings;
        private static readonly CSharpCompilationOptions s_cSharpCompilationOptions;

        private readonly ISet<string?> m_referencedAssemblies;

        static AbstractTest()
        {
            s_verifySettings = new VerifySettings();
            s_verifySettings.UseDirectory("Snapshots");
            s_cSharpCompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable);


            VerifySourceGenerators.Initialize();
            GeneratorDriverResultFilter.Initialize();
        }


        protected AbstractTest(ITestOutputHelper outputHelper)
        {
            m_outputHelper = outputHelper;
            m_viewModules = new List<ViewModule>();
            m_referencedAssemblies = new HashSet<string?>();

            AddAssemblyReference("System");
            AddAssemblyReference("System.Private.CoreLib");
            AddAssemblyReference("System.Linq");
            AddAssemblyReference("netstandard");
            AddAssemblyReference("AutoFactories");
            AddAssemblyReference("System.Runtime");
        }

        /// <summary>
        /// Writes a line of text that will show up in the output of the unit test
        /// </summary>
        /// <param name="text">The text to write</param>
        protected void WriteLine(string text)
        {
            m_outputHelper.WriteLine(text);
        }

        protected void AddAssemblyReference(string name)
        {
            m_referencedAssemblies.Add(name);
        }

        protected void AddAssemblyReference<T>()
        {
            m_referencedAssemblies.Add(typeof(T).Assembly.GetName().Name);
        }

        /// <summary>
        /// Adds a new view module to the template rendere
        /// </summary>
        protected void AddModule<T>() where T : ViewModule, new()
        {
            m_viewModules.Add(new T());
        }

        protected async Task Compose(
            string source,
            string[]? verifySource = null,
            List<string>? notes = null,
            Action<IEnumerable<Diagnostic>>? assertAnalyuzerResult = null)
        {
            List<MetadataReference> references = new List<MetadataReference>();
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName assemblyName = assembly.GetName();
                if(assemblyName.Name is not null && m_referencedAssemblies.Contains(assemblyName.Name))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            SyntaxTree[] syntaxTrees = [CSharpSyntaxTree.ParseText(source)];
            CSharpCompilation baseCompilation = CSharpCompilation.Create("UnitTest", syntaxTrees, references, s_cSharpCompilationOptions);

            // Setup Source Generator 
            AutoFactoriesGenerator generator = new AutoFactoriesGenerator();
            generator.Modules.AddRange(m_viewModules);
            generator.ExceptionHandler += GenerateException;
            AutoFactoriesGeneratorHoist incrementalGenerator = new AutoFactoriesGeneratorHoist(generator);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(incrementalGenerator);
            driver = driver.RunGeneratorsAndUpdateCompilation(baseCompilation, out Compilation finalCompilation, out ImmutableArray<Diagnostic> diagnostics);

            // Do compile check 
            bool hasError = false;
            StringBuilder builder = new StringBuilder()
                .AppendLine("Test case failed as the source code failed to compile");
            foreach (Diagnostic diagnostic in finalCompilation.GetDiagnostics())
            {
                switch (diagnostic.Severity)
                {
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
                WriteLine(source);
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

        private void GenerateException(Exception exception)
        {
            Assert.Fail($"Unhandle exception was thrown by the generator\n{exception}");
        }
    }
}