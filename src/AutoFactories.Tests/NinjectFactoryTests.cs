using Ninject;
using Ninject.Syntax;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class NinjectFactoryTests : BaseFactoryTest
    {
        public NinjectFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddViews(ProjectPaths.NinjectDir / "Views");
            AddAssemblyReference<IKernel>();
        }



        [Fact]
        public Task Auto_Factories_Module_Should_Bind_Factory_To_Interface()
            => CaptureAsync(
                notes: ["The module should have a binding for the IPersonFactory to PersonFactory"],
                verifySource: ["AutoFactoriesModule"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;
                    
                    namespace World
                    {
                        public interface IPerson { get; }
                    
                        [AutoFactory]
                        internal class Person : IPerson {}
                    }
                    """]);
    }
}
