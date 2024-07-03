//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Diagnostics;
//using System.Collections.Immutable;

//namespace Ninject.AutoFactories
//{
//    [DiagnosticAnalyzer(LanguageNames.CSharp)]
//    internal class AutoFactoriesAnalyzer : DiagnosticAnalyzer
//    {
//        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

//        public override void Initialize(AnalysisContext context)
//        {
//            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
//            context.EnableConcurrentExecution();

//            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
//        }

//        private void AnalyzeSymbol(SymbolAnalysisContext context)
//        {

//        }
//    }
//}
