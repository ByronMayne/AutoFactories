using Vogen;

namespace Ninject.AutoFactory
{
    [Instance("Public", "public")]
    [Instance("Internal", "internal")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    public partial struct AccessModifier { }
}
