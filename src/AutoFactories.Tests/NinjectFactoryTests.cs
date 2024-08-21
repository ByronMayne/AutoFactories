using AutoFactories.Ninject;
using Ninject;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class NinjectFactoryTests : FactoryTests
    {
        public NinjectFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddModule<NinjectViewModule>();
            AddAssemblyReference<IKernel>();
            AddAssemblyReference<NinjectViewModule>();
        }
    }
}
