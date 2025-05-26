// -----------------------------| Notes |-----------------------------
// 1. The module should have a binding for the IPersonFactory to PersonFactory
// -------------------------------------------------------------------
using System;
using Ninject.Modules;


public class AutoFactoriesModule : NinjectModule
{
    public override void Load()
    {
        Bind<World.IPersonFactory>().To<World.PersonFactory>();
    }
}