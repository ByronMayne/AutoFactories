using AutoFactories.Types;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;

namespace AutoFactories
{
    internal static class TypeNames
    {
        public static MetadataTypeName ClassAttributeType { get; }
        public static MetadataTypeName ParameterAttributeType { get; }
        public static AccessModifier AttributeAccessModifier { get; }

        static TypeNames()
        {
            AttributeAccessModifier = AccessModifier.Internal;
            ClassAttributeType = new MetadataTypeName("AutoFactories.AutoFactoryAttribute");
            ParameterAttributeType = new MetadataTypeName("AutoFactories.FromFactoryAttribute");
        }
    }
}
