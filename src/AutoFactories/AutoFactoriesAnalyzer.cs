using AutoFactories.Diagnostics;
using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;
using System.Collections.Immutable;
using System.Linq;

namespace AutoFactories
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AutoFactoriesAnalyzer : DiagnosticAnalyzer
    {
        private UnmarkedFactoryDiagnosticBuilder m_unmarkedFactoryDiagnostic;
        private InconsistentFactoryAcessibilityBuilder m_inconsistentFactoryAcessibility;
        private ExposedAsNotDerivedTypeDiagnosticBuilder m_exposedAs;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public AutoFactoriesAnalyzer()
        {
            Options options = new Options();

            m_unmarkedFactoryDiagnostic = new UnmarkedFactoryDiagnosticBuilder(options);
            m_inconsistentFactoryAcessibility = new InconsistentFactoryAcessibilityBuilder();
            m_exposedAs = new ExposedAsNotDerivedTypeDiagnosticBuilder();

            SupportedDiagnostics = [
                m_unmarkedFactoryDiagnostic.Descriptor,
                m_inconsistentFactoryAcessibility.Descriptor,
                m_exposedAs.Descriptor,
             ];
        }


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction((context) =>
            {
                ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
                Options options = new(context.Options.AnalyzerConfigOptionsProvider);
                ClassDeclarationVisitor visitor = new(true, options, context.SemanticModel);
                visitor.VisitClassDeclaration(classDeclarationSyntax);
                Analyze(visitor, context);
            }, SyntaxKind.ClassDeclaration);
        }

        private void Analyze(ClassDeclarationVisitor vistor, SyntaxNodeAnalysisContext context)
        {
            foreach(Diagnostic diagnostic in vistor.GetDiagnostics())
            {
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
