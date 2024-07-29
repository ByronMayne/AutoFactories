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

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public AutoFactoriesAnalyzer()
        {
            Options options = new Options();

            m_unmarkedFactoryDiagnostic = new UnmarkedFactoryDiagnosticBuilder(options);
            m_inconsistentFactoryAcessibility = new InconsistentFactoryAcessibilityBuilder();

            SupportedDiagnostics = [
                m_unmarkedFactoryDiagnostic.Descriptor,
                m_inconsistentFactoryAcessibility.Descriptor,
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
                ClassDeclartionVisitor visitor = new(true, options, context.SemanticModel);
                visitor.VisitClassDeclaration(classDeclarationSyntax);
                Analyze(visitor, context);
            }, SyntaxKind.ClassDeclaration);
        }

        private void Analyze(ClassDeclartionVisitor vistor, SyntaxNodeAnalysisContext context)
        {
            // UnmarkedFactory
            if (!vistor.HasMarkerAttribute)
            {
                vistor.Constructors
                    .SelectMany(c => c.Parameters)
                    .Where(p => p.HasMarkerAttribute)
                    .ReportDiagnostic(context, m_unmarkedFactoryDiagnostic.Build);
            }

            // InconsistentFactoryAcessibility
            if (vistor.AccessModifier == AccessModifier.Internal &&
                vistor.FactoryAccessModifier == AccessModifier.Public)
            {
                context.ReportDiagnostic(m_inconsistentFactoryAcessibility.Build(vistor));
            }
        }
    }
}
