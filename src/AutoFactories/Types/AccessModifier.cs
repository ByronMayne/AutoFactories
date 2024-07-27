using Microsoft.CodeAnalysis;
using Vogen;

namespace Ninject.AutoFactories
{
    [Instance("Public", "public")]
    [Instance("Internal", "internal")]
    [Instance("Private", "private")]
    [Instance("Protected", "protected")]
    [Instance("ProtectedAndInternal", "protected internal")]
    [ValueObject<string>(conversions: Conversions.None, toPrimitiveCasting: CastOperator.Implicit)]
    public partial struct AccessModifier 
    {
        public static AccessModifier FromSymbol(ISymbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                default:
                case Accessibility.Internal: return Internal;
                case Accessibility.Public: return Public; 
                case Accessibility.Protected: return Protected;
                case Accessibility.ProtectedAndInternal: return ProtectedAndInternal;
                case Accessibility.Private: return Private;
            }
        }
    }
}
