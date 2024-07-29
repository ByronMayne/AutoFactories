using Vogen;

namespace AutoFactories.Diagnostics
{
    [Instance("UnmarkedFactory", "AF1001")]
    [Instance("InconsistentFactoryAcessibility", "AF1002")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    internal partial record DiagnosticIdentifier
    {}
}
