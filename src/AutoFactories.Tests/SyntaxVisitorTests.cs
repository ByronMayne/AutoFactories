using Microsoft.CodeAnalysis;
using AutoFactories.Visitors;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FluentAssertions;
using Ninject.AutoFactories;
using Xunit.Abstractions;
using Microsoft.CodeAnalysis.CSharp;
using System.Text;

namespace AutoFactories.Tests
{
    public class SyntaxVisitorTests : AbstractTest
    {
        public SyntaxVisitorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddGenerator<AutoFactoriesGeneratorHoist>();
            AddAnalyzer<AutoFactoriesAnalyzer>();
        }

        [Fact]
        public Task PublicClass_CreatesPublicFactory()
            => Compose("""
                public class MyClass
                {

                }
                """,
                CreatesPublicFactory);

        [Fact]
        public Task InternalClass_CreatesInternalFactory()
            => Compose("""
                internal class MyClass
                {}
                """,
                CreatesInternalFactory);

        [Fact]
        public Task Internal_Class_With_Public_Interface_Creates_Public_Factory()
            => Compose("""
                using AutoFactories;

                public interface IHuman {}
                [AutoFactory(ExposeAs=typeof(IHuman))]
                internal class Human : IHuman {}
                """,
                CreatesPublicFactory);


        private async Task Compose(string source, params Action<ClassDeclarationVisitor>[] asserts)
        {
            UnitTestCompiler compiler = CreateCompiler();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, encoding: Encoding.UTF8);

            CompileResult compileResult = await compiler.CompileAsync([syntaxTree]);

            Diagnostic[] errors = compileResult.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if(errors.Length > 0)
            {
                Assert.Fail($"There were compiler errors\n - {string.Join("\n -", errors.Select(e => e.GetMessage()))}");
            }

            ClassDeclarationVisitor visitor = new ClassDeclarationVisitor(true, new Options(), compileResult.SemanticModel);

            visitor.VisitClassDeclaration(syntaxTree
                .GetRoot()
                .ChildNodes()
                .OfType<ClassDeclarationSyntax>()
                .First());

            foreach (Action<ClassDeclarationVisitor> assert in asserts)
            {
                assert(visitor);
            }
        }

        private static Action<ClassDeclarationVisitor> CreatesPublicFactory => CreateFactoryThatIs(AccessModifier.Public);
        private static Action<ClassDeclarationVisitor> CreatesInternalFactory => CreateFactoryThatIs(AccessModifier.Internal);

        private static Action<ClassDeclarationVisitor> CreateFactoryThatIs(AccessModifier modifier)
            => c => c.FactoryAcessModifier.Should().Be(modifier);
    }
}
