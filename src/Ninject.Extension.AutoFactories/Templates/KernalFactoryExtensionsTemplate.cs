using Ninject.AutoFactory.Templates;

namespace Ninject.Extension.AutoFactories.Templates
{
    internal class KernalFactoryExtensionsTemplate : Template
    {
        public KernalFactoryExtensionsTemplate() : base("Ninject.KernalFactoryExtensions.g.cs")
        { }

        /// <inheritdoc cref="Template"/>
        protected override string Render()
        {
            return $$"""
                #nullable enable

                namespace Ninject
                {
                    /// <summary>
                    /// Contains all the bindings for the generated type factories 
                    /// </summary>
                    internal static class KernalFactoryExtensions
                    {
                        /// <summary>
                        /// Adds the generated factory methods from Ninject.Extensions.AutoFactory
                        /// </summary
                        public static TKernel LoadFactories<TKernel>(this TKernel kernel) where TKernel : global::Ninject.IKernel
                        {
                            kernel.Load(new global::Ninject.AutoFactoriesModule());
                            return kernel;
                        }
                    }
                }
                """;
        }
    }
}
