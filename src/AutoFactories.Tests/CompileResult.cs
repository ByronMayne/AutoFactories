using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace AutoFactories.Tests
{
    internal record CompileResult(SyntaxTree SyntaxTree, Compilation Compilation, SemanticModel SemanticModel, ImmutableArray<Diagnostic> Diagnostics);
}
