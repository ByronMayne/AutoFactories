using Microsoft.CodeAnalysis;
using AutoFactories.Visitors;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FluentAssertions;
using Ninject.AutoFactories;

namespace AutoFactories.Tests
{
    public class SyntaxVisitorTests
    {

        [Fact]
        public void PublicClass_CreatesPublicFactory()
            => Compose("""
                public class MyClass
                {

                }
                """,
                CreatesPublicFactory);

        [Fact]
        public void InternalClass_CreatesInternalFactory()
            => Compose("""
                internal class MyClass
                {}
                """,
                CreatesInternalFactory);

        [Fact]
        public void PublicClass_WithExposedPublicInterface_CreatesPublicFactory()
            => Compose("""
                using AutoFactories;

                public interface IHuman {}
                [AutoFactory(ExposeAs=typeof(IHuman))]
                internal class Human : IHuman {}
                """,
                CreatesPublicFactory);


        private void Compose(string source, params Action<ClassDeclarationVisitor>[] asserts)
        {
            UnitTestCompiler compiler = new UnitTestCompiler();
            CompileResult result = compiler.Compile([source]);

            Diagnostic[] errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .ToArray();

            if(errors.Length > 0)
            {
                Assert.Fail($"There were compiler errors\n - {string.Join("\n -", errors.Select(e => e.GetMessage()))}");
            }

            ClassDeclarationVisitor visitor = new ClassDeclarationVisitor(true, new Options(), result.SemanticModel);

            visitor.VisitClassDeclaration(result.SyntaxTree
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
            => c => c.Accessibility.Should().Be(modifier);
    }
}
