using AutoFactories.Types;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;

namespace AutoFactories
{
    internal class GeneratorOptions
    {
        public MetadataTypeName ClassAttributeType { get; }
        public MetadataTypeName ParameterAttributeType { get; }
        public AccessModifier AttributeAccessModifier { get; }

        public GeneratorOptions()
        {
            AttributeAccessModifier = AccessModifier.Internal;
            ClassAttributeType = new MetadataTypeName("AutoFactories.AutoFactoryAttribute");
            ParameterAttributeType = new MetadataTypeName("AutoFactories.FromFactoryAttribute");
        }

        public GeneratorOptions(AnalyzerConfigOptionsProvider provider) : this()
        {

        }
    }
}
