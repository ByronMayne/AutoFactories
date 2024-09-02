using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;

namespace AutoFactories.Diagnostics
{
    internal static class DiagnosticExtensions
    {
        public static void ReportDiagnostic<T>(this IEnumerable<T> source,
            SyntaxNodeAnalysisContext context,
            Func<T, Diagnostic> factory)
        {
            foreach (T item in source)
            {
                Diagnostic diagnostic = factory(item);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
