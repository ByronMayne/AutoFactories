using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class MicrosoftDITests : BaseFactoryTest
    {
        public MicrosoftDITests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddViews(ProjectPaths.MicrosoftDir / "Views");
            AddAssemblyReference<IServiceCollection>();
        }


        [Fact]
        public Task SharedFactoryOnlyGeneratesOneBinding()
        => CaptureAsync(
            notes: ["Two classes are sharing the same factory, we need to valdiate they only bind once"],
            verifySource: ["AutoFactoryServiceCollectionExtensions"],
            source: ["""
                        using AutoFactories;

                        [AutoFactory(typeof(AnimalFactory), "Cat"]
                        public class Cat {}

                        [AutoFactory(typeof(AnimalFactory), "Dog"]
                        public class Dog {}
                    
                        [AutoFactory(typeof(AnimalFactory), "Bird"]
                        public class Bird {}

                        public partial class AnimalFactory {}

                        """]);
    }
}
