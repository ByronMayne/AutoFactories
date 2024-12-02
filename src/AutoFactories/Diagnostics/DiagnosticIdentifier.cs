using Vogen;

namespace AutoFactories.Diagnostics
{
    [Instance("UnmarkedFactory", "AF1001")]
    [Instance("InconsistentFactoryAcessibility", "AF1002")]
    [Instance("ExposedAsIsNotDerivedType", "AF1003")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    internal partial record DiagnosticIdentifier
    {}
}
