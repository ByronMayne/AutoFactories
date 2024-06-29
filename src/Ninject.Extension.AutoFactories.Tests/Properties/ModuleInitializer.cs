using System.Runtime.CompilerServices;
using VerifyTests;

namespace Ninject.AutoFactories.Properties;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
