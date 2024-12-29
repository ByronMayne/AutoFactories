using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace AutoFactories.Tests
{
    public record CompileResult(Compilation Compilation, SemanticModel SemanticModel, ImmutableArray<Diagnostic> Diagnostics);
}
