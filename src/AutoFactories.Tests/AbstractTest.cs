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
using VerifyTests;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis.Emit;

namespace AutoFactories.Tests
{
    public abstract class AbstractTest
    {
        protected readonly ITestOutputHelper m_outputHelper;
        private static readonly VerifySettings s_verifySettings;

        private static readonly DirectoryInfo s_sourceRoot;

        private readonly ISet<string?> m_referencedAssemblies;
        private readonly List<AdditionalText> m_additionalTexts;

        static AbstractTest()
        {
            s_sourceRoot = GetSourceRoot();
            s_verifySettings = new VerifySettings();
            s_verifySettings.UseDirectory("Snapshots");


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
            Func<int>? assertExitCode = null,
            Action<IEnumerable<Diagnostic>>? assertAnalyuzerResult = null,
            [CallerMemberName] string callerMemberName = "")
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

            SyntaxTree[] syntaxTrees = [CSharpSyntaxTree.ParseText(source, path: "TestSource")];
            OutputKind outputKind = assertExitCode is null
                ? OutputKind.DynamicallyLinkedLibrary
                : OutputKind.WindowsApplication;

            CSharpCompilationOptions cSharpCompilationOptions = new CSharpCompilationOptions(outputKind)
                 .WithNullableContextOptions(NullableContextOptions.Enable);

            CSharpCompilation baseCompilation = CSharpCompilation.Create("UnitTest", syntaxTrees, references, cSharpCompilationOptions);

            // Setup Source Generator 
            AutoFactoriesGenerator generator = new AutoFactoriesGenerator();
            generator.ExceptionHandler += GenerateException;
            AutoFactoriesGeneratorHoist incrementalGenerator = new AutoFactoriesGeneratorHoist(generator);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(incrementalGenerator);
            driver = driver.AddAdditionalTexts(m_additionalTexts.ToImmutableArray());
            driver = driver.RunGeneratorsAndUpdateCompilation(baseCompilation, out Compilation finalCompilation, out ImmutableArray<Diagnostic> diagnostics);



            diagnostics = finalCompilation.GetDiagnostics()
                .ToImmutableArray();

            if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                ReportDiagnostics(finalCompilation, diagnostics);
            }

            // Assert Analyzer
            if (assertAnalyuzerResult != null)
            {
                AnalysisResult analysisResults = await finalCompilation.WithAnalyzers([new AutoFactoriesAnalyzer()])
                    .GetAnalysisResultAsync(CancellationToken.None);

                assertAnalyuzerResult(analysisResults.GetAllDiagnostics());
            }


            if (assertExitCode != null)
            {
                using (Stream assemblyStream = new MemoryStream())
                using (Stream assemblySymbolStream = new MemoryStream())
                {
                    EmitOptions emitOptions = new EmitOptions();
                    EmitResult emitResult = finalCompilation.Emit(assemblyStream, assemblySymbolStream, null, null, null, emitOptions, null, null, null, CancellationToken.None);
                    Assert.True(emitResult.Success, "Failed to emit the generated assembly");
                    assemblyStream.Position = 0;
                    assemblySymbolStream.Position = 0;

                    AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext(callerMemberName, true);
                    try
                    {
                        Assembly assembly = assemblyLoadContext.LoadFromStream(assemblyStream, assemblySymbolStream);
                        MethodInfo? entryPoint = assembly.EntryPoint;
                        Assert.NotNull(entryPoint);
                        object? result = entryPoint.Invoke(null, Array.Empty<string>());
                        Assert.NotNull(result);
                        if (result is int exitCode)
                        {
                            Assert.Equal(assertExitCode(), exitCode);
                        }

                    }
                    catch (Exception exception)
                    {
                        Assert.Fail(exception.Message);
                    }
                    finally
                    {
                        assemblyLoadContext.Unload();
                    }
                    //AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext("UnitTests");
                    //assemblyLoadContext.LoadFromStream(finalCompilation.emi)

                }
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

        private void ReportDiagnostics(Compilation compilation, ImmutableArray<Diagnostic> diagnostics)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("## Source Files");
            foreach (SyntaxTree syntaxTree in compilation.SyntaxTrees)
            {
                builder.AppendLine($"### `{Path.GetFileName(syntaxTree.FilePath)}`");

                Diagnostic[] treeDiagnostics = diagnostics
                    .Where(d => d.Location.SourceTree == syntaxTree)
                    .Where(d => d.Severity > DiagnosticSeverity.Info)
                    .OrderBy(d => d.Location.SourceSpan.Start)
                    .ToArray();

                if (treeDiagnostics.Any())
                {
                    builder.AppendLine("#### Diagnostic");
                    foreach (Diagnostic diagnostic in treeDiagnostics)
                    {
                        FileLinePositionSpan fileLinePosition = diagnostic.Location.GetLineSpan();
                        var startPosition = fileLinePosition.StartLinePosition;
                        builder.AppendLine($" * {diagnostic.Severity} `{diagnostic.Id}` [{startPosition.Line},{startPosition.Character}]: {diagnostic.GetMessage()}");
                    }
                }
                builder.AppendLine("```cs");
                string[] lines = syntaxTree.ToString()
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                {
                    string lineNumber = $"{i + 1}.".PadRight(4);
                    builder.Append(lineNumber);
                    builder.AppendLine(lines[i]);
                }
                builder.AppendLine("```");

            }
            
            m_outputHelper.WriteLine(builder.ToString());
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