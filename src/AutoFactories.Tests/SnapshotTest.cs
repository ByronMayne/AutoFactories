using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public abstract class SnapshotTest : AbstractTest
    {
        private readonly VerifySettings m_verifySettings;

        protected SnapshotTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            m_verifySettings = new VerifySettings();
            m_verifySettings.UseDirectory("Snapshots");
            m_verifySettings.UseTypeName(GetType().Name);
        }

        /// <summary>
        /// Captures a snapshot of the source <see cref="SyntaxTree"/> that are
        /// generated
        /// </summary>
        /// <param name="notes">Optional notes to add to the generated snapshot</param>
        /// <param name="source">The source code toa dd</param>
        /// <param name="verifySource">The qualifed names of the source files to generate the snapshots for</param>
        /// <returns></returns>
        public async Task CaptureAsync(
            string[]? notes = null,
            string[]? source = null,
            string[]? verifySource = null)
        {
            notes ??= Array.Empty<string>();
            source ??= Array.Empty<string>();
            verifySource ??= Array.Empty<string>();

            SyntaxTree[] syntaxTress = source
                .Select((text, i) => SourceText.From(text, Encoding.UTF8))
                .Select((source, i) => CSharpSyntaxTree.ParseText(source, path: $"Source:{i + 1}"))
                .ToArray();

            UnitTestCompiler compiler = CreateCompiler();
            CompileResult compileResult = await compiler.CompileAsync(syntaxTress);
            Compilation compilation = compileResult.Compilation;


            Dictionary<string, IList<SyntaxTree>> treeMap = new Dictionary<string, IList<SyntaxTree>>();

            foreach (SyntaxTree syntaxTree in compileResult.Compilation.SyntaxTrees)
            {
                string typeName = GetTypeName(syntaxTree, compilation);
                if (!treeMap.TryGetValue(typeName, out IList<SyntaxTree>? trees))
                {
                    trees = new List<SyntaxTree>();
                    treeMap[typeName] = trees;
                }

                trees.Add(syntaxTree);
            }

            // Verify that all the types requested exist
            AssertMissingTargets(verifySource, treeMap.Keys.ToArray());

            foreach (KeyValuePair<string, IList<SyntaxTree>> sourceTarget in treeMap)
            {
                if (!verifySource.Contains(sourceTarget.Key))
                {
                    continue;
                }

                StringBuilder dataBuilder = new();
                if (notes.Length > 0)
                {
                    dataBuilder.AppendLine("// -----------------------------| Notes |-----------------------------");
                    for (int i = 0; i < notes.Length; i++)
                    {
                        dataBuilder.AppendFormat("// {0}. {1}", i + 1, notes[i]).AppendLine();
                    }
                    dataBuilder.AppendLine("// -------------------------------------------------------------------");
                }

                for (int i = 0; i < sourceTarget.Value.Count; i++)
                {
                    SyntaxTree syntaxTree = sourceTarget.Value[i];
                    dataBuilder.AppendLine(syntaxTree.ToString());
                    
                    if(i +1 < sourceTarget.Value.Count)
                    {
                        dataBuilder.AppendLine("// -------------------------------------------------------------------");
                    }
                }

                Target target = new("cs", dataBuilder.ToString().Trim(), sourceTarget.Key);

                VerifySettings settings = new(m_verifySettings);
                settings.UseTypeName($"{GetType().Name}_{sourceTarget.Key}");
                await Verify(target, settings);
            }
        }

        private void AssertMissingTargets(string[] expected, string[] actual)
        {
            if (expected.Length == 0)
            {
                return;
            }

            List<string> missing = new List<string>();
            foreach (string e in expected)
            {
                if (!actual.Contains(e))
                {
                    missing.Add(e);
                }
            }

            if(missing.Count == 0)
            {
                return;
            }

            m_outputHelper.WriteLine("Missing Sources:");
            foreach (string m in missing)
            {
                m_outputHelper.WriteLine($" - {m}");
            }

            m_outputHelper.WriteLine("Found Sources:");
            foreach (string a in actual)
            {
                m_outputHelper.WriteLine($" - {a}");
            }

            Assert.Fail("The test required sources be verified but they were not found");
        }

        private static string GetTypeName(SyntaxTree syntaxTree, Compilation compilation)
        {
            SyntaxNode root = syntaxTree.GetRoot();
            TypeDeclarationSyntax? classDeclaration =
                root.DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .FirstOrDefault();

            SemanticModel semanticModel = compilation.GetSemanticModel(syntaxTree);
            INamedTypeSymbol? typeSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);

            return typeSymbol is null
                ? throw new Exception($"Unable to get the type name for the tree {syntaxTree}")
                : typeSymbol.ToDisplayString();
        }
    }
}