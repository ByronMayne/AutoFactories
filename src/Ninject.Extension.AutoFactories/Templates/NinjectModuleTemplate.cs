using Ninject.AutoFactory.Models;
using Ninject.Extension.AutoFactories;
using Ninject.Extension.AutoFactories.Models;
using System.Collections.Immutable;

namespace Ninject.AutoFactory.Templates
{
    internal class NinjectModuleTemplate : Template
    {
        private readonly IReadOnlyList<FactoryModel> m_models;

        public NinjectModuleTemplate(IReadOnlyList<FactoryModel> models) : base("Ninject.FactoryModule.g.cs")
        {
            m_models = models;
        }

        /// <inheritdoc cref="Template"/>
        protected override string Render()
        {
            return $$"""
                #nullable enable

                namespace {{GeneratorSettings.NinjectModule.Namespace}}
                {
                    /// <summary>
                    /// Contains all the bindings for the generated type factories 
                    /// </summary>
                    internal sealed class {{GeneratorSettings.NinjectModule.TypeName}} : global::Ninject.Modules.NinjectModule 
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
            ClassWriter writer = new ClassWriter(3);

            foreach(FactoryModel model in m_models)
            {
                writer.WriteLine($"Bind<global::{model.InterfaceType.FullName}>().To<global::{model.Type.FullName}>().InSingletonScope();");
            }

            return writer.ToString();
        }
    }
}
