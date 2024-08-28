using Ninject;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class NinjectFactoryTests : FactoryTests
    {
        public NinjectFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddViews("AutoFactories.Ninject\\Views");
            AddAssemblyReference<IKernel>();
        }
    }
}
