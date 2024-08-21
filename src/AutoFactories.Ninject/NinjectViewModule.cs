using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFactories.Ninject
{
    internal class NinjectViewModule : ViewModule
    {
        public NinjectViewModule() : base("Ninject")
        {
        }

        public override void Load()
        {
            SetPartial(PartialName.FactoryMethod, "Partials\\FactoryMethod.hbs");
            SetPartial(PartialName.FactoryConstructor, "Partials\\FactoryConstructor.hbs");
            SetPartial(PartialName.FactoryNamespaces, "Partials\\FactoryNamespaces.hbs");
        }
    }
}
