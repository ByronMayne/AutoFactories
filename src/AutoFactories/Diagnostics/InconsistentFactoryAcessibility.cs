using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;

namespace AutoFactories.Diagnostics
{
    internal class InconsistentFactoryAcessibilityBuilder : DiagnosticBuilder
    {
        public InconsistentFactoryAcessibilityBuilder() : base(
            id: DiagnosticIdentifier.InconsistentFactoryAcessibility,
            title: "Inconsistent Factory Acessibility",
            category: "Code",
            messageFormat: 
                $"The type '{{0}}' is marked as internal but the factory {{1}} is public. Either change {{0}} to public or change {{1}} to internal.")
        {
            Severity = DiagnosticSeverity.Error;
        }

        public Diagnostic Build(ClassDeclartionVisitor visitor)
            => Diagnostic.Create(Descriptor,
                visitor.FactoryTypeLocation,
                new object?[] { visitor.Type.QualifiedName, visitor.FactoryType.QualifiedName });

    }
}
