using Vogen;

namespace AutoFactories
{
    [Instance("MethodParameters", "MethodParameters", "Used to render the parameters for a method")]
    [Instance("FactoryConstructor", "FactoryConstructors", "Used to render the constructor for the factory instance.")]
    [Instance("FactoryProperties", "FactoryProperties", "Used to render extra properties for both the instance and interface of the factories.")]
    [Instance("FactoryFields", "FactoryFields", "Used to render fields for just the factory instance.")]
    [Instance("FactoryMethod", "FactoryMethod", "Used to generate the methods that create the instances")]
    [Instance("FactoryNamespaces", "FactoryNamespaces", "Imported at the type of factories and it's interface to include namespaces")]
    [ValueObject<string>(conversions: Conversions.None)]
    public readonly partial struct PartialResourceKey
    { }
}
