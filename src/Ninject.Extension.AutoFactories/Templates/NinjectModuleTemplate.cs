using Ninject.AutoFactories.Models;

namespace Ninject.AutoFactories.Templates
{
    internal class NinjectModuleTemplate : Template
    {
        private readonly string m_assemblyName;
        private readonly IReadOnlyList<FactoryModel> m_models;

        public NinjectModuleTemplate(string assemblyName, IReadOnlyList<FactoryModel> models) : base("Ninject.FactoryModule.g.cs")
        {
            m_models = models;
            m_assemblyName = assemblyName;
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
                    internal sealed partial class {{GeneratorSettings.NinjectModule.TypeName}} : global::Ninject.Modules.NinjectModule 
                    {
                        /// <inheritdoc cref=" global::Ninject.Modules.NinjectModule "/>
                        public override string Name { get; }
                        
                        public {{GeneratorSettings.NinjectModule.TypeName}}()
                        {
                            Name = "{{m_assemblyName}}.AutoFactoriesModule";
                        }

                        /// <inheritdoc cref=" global::Ninject.Modules.NinjectModule "/>
                        public override void Load()
                        {
                            {{WriteBindings()}}
                            AfterBindings();
                        }

                        /// <summary>
                        /// Callback that is invoked after all bindings have been applied
                        /// </summary>
                        partial void AfterBindings();
                    }
                }
                """;
        }

        private string WriteBindings()
        {
            ClassWriter writer = new(3);

            foreach (FactoryModel model in m_models)
            {
                _ = writer.WriteLine($"Bind<global::{model.InterfaceType.FullName}>().To<global::{model.Type.FullName}>().InSingletonScope();");
            }

            return writer.ToString();
        }
    }
}
