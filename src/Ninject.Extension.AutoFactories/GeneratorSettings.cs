namespace Ninject.AutoFactories
{

    /// <summary>
    /// Contains the settings used by the generator
    /// </summary>
    internal static class GeneratorSettings
    {
        public static MetadataTypeName ClassAttribute { get; }
        public static MetadataTypeName ParameterAttribute { get; }
        public static MetadataTypeName NinjectModule { get; }

        static GeneratorSettings()
        {
            NinjectModule = new MetadataTypeName("Ninject.AutoFactoriesModule");
            ClassAttribute = new MetadataTypeName("Ninject.GenerateFactoryAttribute");
            ParameterAttribute = new MetadataTypeName("Ninject.FromFactoryAttribute");
        }
    }
}
