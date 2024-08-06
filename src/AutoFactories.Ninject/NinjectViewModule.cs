using System;
using System.Collections.Generic;
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
            SetView(TemplateName.FactoryMethod, "Views/Partials/FactoryMethod.hbs");
            SetView(TemplateName.FactoryConstructor, "Views/Partials/FactoryConstructor.hbs");

        }
    }
}
