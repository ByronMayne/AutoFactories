using Microsoft.CodeAnalysis;

namespace AutoFactories.Diagnostics
{
    internal abstract class SyntaxDiagnosticBuilder<T> : DiagnosticBuilder
    {
        protected SyntaxDiagnosticBuilder(DiagnosticIdentifier id, string title, string category, string messageFormat) 
            : base(id, title, category, messageFormat)
        {
        }

        public abstract Diagnostic Create(T syntaxNode);
    }


}
