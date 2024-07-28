using AutoFactories.Types;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;

namespace AutoFactories
{
    internal class Options
    {
        public MetadataTypeName ClassAttributeType { get; }
        public MetadataTypeName ParameterAttributeType { get; }
        public AccessModifier AttributeAccessModifier { get; }

        public Options()
        {
            AttributeAccessModifier = AccessModifier.Internal;
            ClassAttributeType = new MetadataTypeName("AutoFactories.AutoFactoryAttribute");
            ParameterAttributeType = new MetadataTypeName("AutoFactories.FromFactoryAttribute");
        }

        public Options(AnalyzerConfigOptionsProvider provider) : this()
        {

        }
    }
}
