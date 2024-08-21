using System;
using System.Collections.Generic;
using System.Text;
using Vogen;

namespace AutoFactories
{

    [Instance("Factory", "factory")]
    [Instance("FactoryInterface", "factory_interface")]
    [Instance("ClassAttribute", "class_attribute")]
    [Instance("ParameterAttribute", "parameter_attribute")]
    [ValueObject<string>(conversions: Conversions.None)]
    public partial struct TemplateName
    {
    }


    [Instance("MethodParameters", "method_parameters", "Used to render the parameters for a method")]
    [Instance("FactoryConstructor", "factory_constructors", "Used to render the constructor for the factory instance.")]
    [Instance("FactoryProperties", "factory_properties", "Used to render extra properties for both the instance and interface of the factories.")]
    [Instance("FactoryFields", "factory_fields", "Used to render fields for just the factory instance.")]
    [Instance("FactoryMethod", "factory_method", "Used to generate the methods that create the instances")]
    [Instance("FactoryNamespaces", "factory_namespaces", "Imported at the type of factories and it's interface to include namespaces")]
    [ValueObject<string>(conversions: Conversions.None)]
    public partial struct PartialName
    {}
}
