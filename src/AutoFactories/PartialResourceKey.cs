using Vogen;

namespace AutoFactories
{
    [Instance("MethodParameters", "Partials/MethodParameters.hbs", "Used to render the parameters for a method")]
    [Instance("FactoryConstructor", "Partials/FactoryConstructors.hbs", "Used to render the constructor for the factory instance.")]
    [Instance("FactoryProperties", "Partials/FactoryProperties.hbs", "Used to render extra properties for both the instance and interface of the factories.")]
    [Instance("FactoryFields", "Partials/FactoryFields.hbs", "Used to render fields for just the factory instance.")]
    [Instance("FactoryMethod", "Partials/FactoryMethod.hbs", "Used to generate the methods that create the instances")]
    [Instance("FactoryNamespaces", "Partials/FactoryNamespaces.hbs", "Imported at the type of factories and it's interface to include namespaces")]
    [ValueObject<string>(conversions: Conversions.None)]
    public readonly partial struct PartialResourceKey
    { }
}
