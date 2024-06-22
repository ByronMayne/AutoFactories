using Ninject.AutoFactory.Models;
using System.Collections.Immutable;

namespace Ninject.AutoFactory.Templates
{
    internal class NinjectModuleTemplate : Template
    {
        private readonly IImmutableList<FactoryModel> m_models;

        public NinjectModuleTemplate(ImmutableArray<FactoryModel> models) : base("Ninject.FactoryModule.g.cs")
        {
            m_models = models;
        }

        /// <inheritdoc cref="Template"/>
        protected override string Render()
        {
            return $$"""
                namespace Ninject
                {
                    /// <summary>
                    /// Contains all the bindings for the generated type factories 
                    /// </summary>
                    internal sealed class FactoryModule : global::Ninject.Modules.NinjectModule 
                    {
                        public override void Load()
                        {
                            {{WriteBindings()}}
                        }
                    }
                }
                """;
        }

        private string WriteBindings()
        {
            ClassWriter writer = new ClassWriter(2);

            foreach(FactoryModel model in m_models)
            {
                writer.WriteLine($"Bind<global::{model.Namespace}.{model.FactoryInterfaceTypeName}>().To<global::{model.Namespace}.{model.FactoryTypeName}>().InSingletonScope();");
            }

            return writer.ToString();
        }
    }
}
