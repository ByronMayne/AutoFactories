using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class MicrosoftDITests : BaseFactoryTest
    {
        public MicrosoftDITests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddViews("AutoFactories.Microsoft.DependencyInjection\\Views");
            AddAssemblyReference<IServiceCollection>();
        }
    }
}
