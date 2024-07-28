using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;

namespace AutoFactories.Diagnostics
{
    internal class UnmarkedFactoryDiagnosticBuilder : DiagnosticBuilder
    {
        public UnmarkedFactoryDiagnosticBuilder(GeneratorOptions options) : base(
            id: 1000,
            title: "Unmarked Factory",
            category: "Code",
            messageFormat: 
                $"The parameter '{{0}}' is marked with the [{options.ParameterAttributeType.Name}] but the declaring class '{{1}}' is " +
                $"missing [{options.ClassAttributeType.Name}]. Either remove [{options.ParameterAttributeType.Name}] from the parameter" +
                  $" or add [{options.ClassAttributeType.Name}] to the declaring class '{{1}}'.")
        {
            Severity = DiagnosticSeverity.Warning;
        }

        public Diagnostic Build(ParameterSyntaxVisitor parameter)
            => Diagnostic.Create(Descriptor, parameter.AttributeLocation, new object?[] { parameter.Name, parameter.Constructor.Class.Type.Name });

    }
}
