using AutoFactories.Diagnostics;
using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AutoFactories
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AutoFactoriesAnalyzer : DiagnosticAnalyzer
    {
        internal UnmarkedFactoryDiagnosticBuilder UnmarkedFactoryDiagnostic { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        public AutoFactoriesAnalyzer()
        {
            Options options = new Options();

            UnmarkedFactoryDiagnostic = new UnmarkedFactoryDiagnosticBuilder(options);

            SupportedDiagnostics = [
                UnmarkedFactoryDiagnostic.Descriptor
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
            // Check for missing atribute
            if (!vistor.HasMarkerAttribute)
            {
                vistor.Constructors
                    .SelectMany(c => c.Parameters)
                    .Where(p => p.HasMarkerAttribute)
                    .ReportDiagnostic(context, UnmarkedFactoryDiagnostic.Build);
            }
        }
    }
}
