namespace Ninject.AutoFactories
{

    /// <summary>
    /// Contains the settings used by the generator
    /// </summary>
    internal static class GeneratorSettings
    {
        public static MetadataTypeName ParameterAttribute { get; }
        public static MetadataTypeName NinjectModule { get; }
        public static ClassAttributeSettings ClassAttribute { get; }

        static GeneratorSettings()
        {
            ClassAttribute = new ClassAttributeSettings();

            NinjectModule = new MetadataTypeName("Ninject.AutoFactoriesModule");
            ParameterAttribute = new MetadataTypeName("Ninject.FromFactoryAttribute");
        }

    }

    internal class ClassAttributeSettings
    {
        public MetadataTypeName Type { get; }

        public string MethodNamePropertyName { get; }
        public string MethodNameArgumentName { get; }
        public string FactoryStringArgumentName { get; }
        public string FactoryTypeArgumentName { get; }
        public string FactoryFullyQualifiedNamePropertyName { get; }

        public ClassAttributeSettings()
        {
            Type = new MetadataTypeName("Ninject.GenerateFactoryAttribute");
            MethodNamePropertyName = "MethodName";
            MethodNameArgumentName = "methodName";
            FactoryStringArgumentName = "factoryFullyQualifiedName";
            FactoryTypeArgumentName = "factoryType";
            FactoryFullyQualifiedNamePropertyName = "FactoryFullyQualifiedName";
        }
    }
}
