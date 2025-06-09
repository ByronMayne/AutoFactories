using Ninject;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class GenericFactoryTests : BaseFactoryTest
    {
        public GenericFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            // Not need to bind anything 
        }

        [Fact]
        public Task Type_With_Shared_Factory_And_Custom_Name()
         => CaptureAsync(
             notes: ["The Factory should have a method called `MakeHuman`"],
             verifySource: ["World.Factory"],
             source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                namespace World
                {
                    public partial class Factory 
                    {}

                    [AutoFactory(typeof(Factory), "MakeHuman")]
                    public class Person 
                    {
                        public Person([FromFactory] IEqualityComparer<string?> comparer)
                        {}
                    }
                }
                """]);
    }
}
