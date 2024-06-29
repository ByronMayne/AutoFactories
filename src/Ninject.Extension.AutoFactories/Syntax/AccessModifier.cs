using Vogen;

namespace Ninject.AutoFactories
{
    [Instance("Public", "public")]
    [Instance("Internal", "internal")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    public partial struct AccessModifier { }
}
