using Vogen;

namespace AutoFactories
{
    [Instance("Factory", "FactoryView.hbs")]
    [Instance("FactoryInterface", "FactoryInterfaceView.hbs")]
    [Instance("ClassAttribute", "ClassAttribute.hbs")]
    [Instance("ParameterAttribute", "ParameterAttribute.hbs")]
    [ValueObject<string>(conversions: Conversions.None)]
    public readonly partial struct ViewResourceKey
    {
    }
}
