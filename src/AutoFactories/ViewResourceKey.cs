using Vogen;

namespace AutoFactories
{
    [Instance("Factory", "FactoryView")]
    [Instance("FactoryInterface", "FactoryInterfaceView")]
    [Instance("ClassAttribute", "ClassAttribute")]
    [Instance("ParameterAttribute", "ParameterAttribute")]
    [ValueObject<string>(conversions: Conversions.None)]
    public readonly partial struct ViewResourceKey
    {
    }
}
