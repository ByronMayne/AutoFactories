using Vogen;

namespace AutoFactories.Diagnostics
{
    [Instance("UnmarkedFactory", "AF1001")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    internal partial record DiagnosticIdentifier
    {}
}
