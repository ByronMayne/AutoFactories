using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;

namespace AutoFactories.Diagnostics
{
    internal class InconsistentFactoryAccessibilityBuilder : DiagnosticBuilder
    {
        public InconsistentFactoryAccessibilityBuilder() : base(
            id: DiagnosticIdentifier.InconsistentFactoryAccessibility,
            title: "Inconsistent Factory Accessibility",
            category: "Code",
            messageFormat:
                $"The type '{{0}}' is marked as internal but the factory {{1}} is public. Either change {{0}} to public or change {{1}} to internal.")
        {
            Severity = DiagnosticSeverity.Error;
        }

        public static Diagnostic Create(ClassDeclarationVisitor visitor)
        {
            InconsistentFactoryAccessibilityBuilder builder = new InconsistentFactoryAccessibilityBuilder();
            return Diagnostic.Create(builder.Descriptor,
                visitor.FactoryTypeLocation,
                new object?[] { visitor.Type.QualifiedName, visitor.FactoryType.QualifiedName });
        }

    }
}
