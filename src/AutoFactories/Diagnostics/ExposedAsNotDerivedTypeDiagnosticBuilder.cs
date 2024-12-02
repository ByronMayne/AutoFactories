using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFactories.Diagnostics
{
    internal class ExposedAsNotDerivedTypeDiagnosticBuilder : DiagnosticBuilder
    {
        public ExposedAsNotDerivedTypeDiagnosticBuilder() : base(
             id: DiagnosticIdentifier.ExposedAsIsNotDerivedType,
             title: "Exposed As Not Derived Type",
             category: "Code",
             messageFormat:
                "The factory for type {0} used the ExposeAs with the type {1} however this type is no derived. The exposed" +
                " type must be the same type, a base class, or common interface."
            )
        {
            Severity = DiagnosticSeverity.Error;
        }

        public Diagnostic Build(
            Location? location,
            INamedTypeSymbol type,
            INamedTypeSymbol expose)
            => Diagnostic.Create(Descriptor, location, new object[] { type.ToDisplayString(), expose.ToDisplayString() });
    }
}
