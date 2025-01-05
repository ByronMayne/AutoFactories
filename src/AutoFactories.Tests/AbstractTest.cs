using AutoFactories.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Ninject.AutoFactories;
using Seed.IO;
using SGF;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{

    public abstract class AbstractTest
    {
        protected readonly ITestOutputHelper m_outputHelper;

        private readonly ISet<MetadataReference> m_references;
        private readonly List<AdditionalText> m_additionalTexts;
        private readonly List<DiagnosticAnalyzer> m_analyzers;
        private readonly List<IIncrementalGenerator> m_generators;

        protected AbstractTest(ITestOutputHelper outputHelper)
        {
            m_outputHelper = outputHelper;
            m_references = new HashSet<MetadataReference>();
            m_additionalTexts = new List<AdditionalText>();
            m_generators = new List<IIncrementalGenerator>();
            m_analyzers = new List<DiagnosticAnalyzer>();

            AddViews(ProjectPaths.AutoFactoriesProjectDir / "Views");

            AddAssemblyReference("System");
            AddAssemblyReference("System.Private.CoreLib");
            AddAssemblyReference("System.Linq");
            AddAssemblyReference("netstandard");
            AddAssemblyReference("AutoFactories");
            AddAssemblyReference("System.Runtime");
        }

        /// <summary>
        /// Adds the view from the given directory. This will override the default ones.
        /// </summary>
        protected void AddViews(AbsolutePath viewDirectory)
        {
            if (!Directory.Exists(viewDirectory))
            {
                throw new DirectoryNotFoundException($"Unable to find directory '{viewDirectory}'");
            }
                m_additionalTexts.AddRange(
                    Directory.GetFiles(viewDirectory, "*.hbs", SearchOption.AllDirectories)
                    .Select(path => new ViewResourceText(path, File.ReadAllText(path))));
        }


        /// <summary>
        /// Adds a new <see cref="MetadataReference"/> for the assembly with the given name.
        /// </summary>
        /// <param name="assemblyName">The full or partial name of the assembly to add the refernece to</param>
        protected void AddAssemblyReference(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly is not null)
            {
                m_references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        /// <summary>
        /// Adds a new <see cref="MetadataReference"/> for the assembly of the given assembly for which the type is contained"/>
        /// </summary>
        /// <typeparam name="T">The types who's assembly you want to add a refernece tood</typeparam>
        protected void AddAssemblyReference<T>()
        {
            m_references.Add(MetadataReference.CreateFromFile(typeof(T).Assembly.Location));
        }

        /// <summary>
        /// Adds a new <see cref="IIncrementalGenerator"/> to the list of generators that will run
        /// </summary>
        /// <typeparam name="T">The type of the generator</typeparam>
        protected void AddGenerator<T>() where T : IIncrementalGenerator, new()
            => m_generators.Add(new T());

        /// <summary>
        /// Adds a new generator to the list of generators that will run
        /// </summary>
        /// <param name="generator"></param>
        protected void AddGenerator(IIncrementalGenerator generator)
        {
            if(generator is IncrementalGenerator incrementalGenerator)
            {
                incrementalGenerator.ExceptionHandler += exception =>
                {
                    Assert.Fail($"Failed due to unhandled exception: {exception}");
                };
            }
            m_generators.Add(generator);
        }

        private void IncrementalGenerator_ExceptionHandler(Exception obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new <see cref="DiagnosticAnalyzer"/> to the list of analyzers that will run
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected void AddAnalyzer<T>() where T : DiagnosticAnalyzer, new()
            => m_analyzers.Add(new T());

        protected UnitTestCompiler CreateCompiler()
            => new UnitTestCompiler(m_generators, m_analyzers, m_additionalTexts);

        protected async Task Compose(
            string source,
            List<string>? notes = null,
            Action<IEnumerable<Diagnostic>>? assertAnalyzerResult = null,
            [CallerMemberName] string callerMemberName = "")
        {
            List<MetadataReference> references = new List<MetadataReference>();

            SyntaxTree[] syntaxTrees = [
                CSharpSyntaxTree.ParseText(SourceText.From(source, Encoding.UTF8), path: callerMemberName)];


            UnitTestCompiler compiler = CreateCompiler();
            CompileResult compileResult = await compiler.CompileAsync(syntaxTrees);

            if (compileResult.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                ReportDiagnostics(compileResult.Compilation, compileResult.Diagnostics);
            }

            // Assert Analyzer
            if (assertAnalyzerResult != null)
            { 
                assertAnalyzerResult(compileResult.Diagnostics);
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
    }
}