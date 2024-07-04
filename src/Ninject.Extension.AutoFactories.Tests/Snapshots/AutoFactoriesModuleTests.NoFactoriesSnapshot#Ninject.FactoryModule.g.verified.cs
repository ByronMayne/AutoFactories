//HintName: Ninject.FactoryModule.g.cs
#nullable enable

namespace Ninject
{
    /// <summary>
    /// Contains all the bindings for the generated type factories 
    /// </summary>
    internal sealed partial class AutoFactoriesModule : global::Ninject.Modules.NinjectModule 
    {
        /// <inheritdoc cref=" global::Ninject.Modules.NinjectModule "/>
        public override string Name { get; }
        
        public AutoFactoriesModule()
        {
            Name = "AutoFactoriesTests.AutoFactoriesModule";
        }

        /// <inheritdoc cref=" global::Ninject.Modules.NinjectModule "/>
        public override void Load()
        {
            
            AfterBindings();
        }

        /// <summary>
        /// Callback that is invoked after all bindings have been applied
        /// </summary>
        partial void AfterBindings();
    }
}