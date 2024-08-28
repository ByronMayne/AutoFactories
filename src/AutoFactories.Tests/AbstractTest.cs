using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Ninject.AutoFactories;
using Xunit.Abstractions;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Text;
using System.Reflection;
using AutoFactories.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AutoFactories.Tests
{
    public abstract class AbstractTest
    {
        protected readonly ITestOutputHelper m_outputHelper;
        private static readonly VerifySettings s_verifySettings;
        private static readonly CSharpCompilationOptions s_cSharpCompilationOptions;
        private static readonly DirectoryInfo s_sourceRoot;

        private readonly ISet<string?> m_referencedAssemblies;
        private readonly List<AdditionalText> m_additionalTexts;

        static AbstractTest()
        {
            s_sourceRoot = GetSourceRoot();
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
            m_referencedAssemblies = new HashSet<string?>();
            m_additionalTexts = new List<AdditionalText>();

            AddViews("AutoFactories\\Views");

            AddAssemblyReference("System");
            AddAssemblyReference("System.Private.CoreLib");
            AddAssemblyReference("System.Linq");
            AddAssemblyReference("netstandard");
            AddAssemblyReference("AutoFactories");
            AddAssemblyReference("System.Runtime");
        }

        /// <summary>
        /// Adds the view from the given path
        /// </summary>
        protected void AddViews(string relativePath)
        {
            string directory = Path.Combine(s_sourceRoot.FullName, relativePath);
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException($"Unable to find directory '{directory}'");
            }
            m_additionalTexts.AddRange(
                Directory.GetFiles(directory, "*.hbs", SearchOption.AllDirectories)
                .Select(path => new HandlebarsText(path, File.ReadAllText(path))));
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

        protected async Task Compose(
            string source,
            string[]? verifySource = null,
            List<string>? notes = null,
            Action<IEnumerable<Diagnostic>>? assertAnalyuzerResult = null)
        {
            List<MetadataReference> references = new List<MetadataReference>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName assemblyName = assembly.GetName();
                if (assemblyName.Name is not null && m_referencedAssemblies.Contains(assemblyName.Name))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            SyntaxTree[] syntaxTrees = [CSharpSyntaxTree.ParseText(source)];
            CSharpCompilation baseCompilation = CSharpCompilation.Create("UnitTest", syntaxTrees, references, s_cSharpCompilationOptions);

            // Setup Source Generator 
            AutoFactoriesGenerator generator = new AutoFactoriesGenerator();
            generator.ExceptionHandler += GenerateException;
            AutoFactoriesGeneratorHoist incrementalGenerator = new AutoFactoriesGeneratorHoist(generator);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(incrementalGenerator);
            driver = driver.AddAdditionalTexts(m_additionalTexts.ToImmutableArray());
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

        private static DirectoryInfo GetSourceRoot([CallerFilePath] string filePath = "")
        {
            DirectoryInfo? directoryInfo = new DirectoryInfo(filePath);

            while (directoryInfo is not null &&
                !Path.Exists(Path.Combine(directoryInfo.FullName, "AutoFactories.sln")))
            {
                directoryInfo = directoryInfo.Parent;
            }
            return directoryInfo ?? throw new Exception("Unable to find the source directory");
        }
    }
}