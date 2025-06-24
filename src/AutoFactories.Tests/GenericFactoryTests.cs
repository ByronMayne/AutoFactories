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
        public Task Type_With_Name_Of_Attribute_Genertes()
            => CaptureAsync(
                notes: ["Create should have the name 'StringComparer'"],
                verifySource: ["World.Factory"],
                source: ["""
                    using AutoFactories;
                    using System.Collections.Generic;

                    namespace World
                    {
                        public partial class Factory 
                        {}

                        [AutoFactory(typeof(Factory), $"{nameof(System.StringComparer)}")]
                        public class Person 
                        {
                            public Person([FromFactory] IEqualityComparer<string?> comparer)
                            {}
                        }
                    }
                    """]);


        [Fact]
        public Task Factory_Method_Name_Using_InvocationExpression_Produces_Expected()
         => CaptureAsync(
             notes: ["The Factory should have a method called `Person`"],
             verifySource: ["World.Factory"],
             source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                namespace World
                {
                    public partial class Factory 
                    {}

                    [AutoFactory(typeof(Factory), nameof(Person))]
                    public class Person 
                    {
                        public Person([FromFactory] IEqualityComparer<string?> comparer)
                        {}
                    }
                }
                """]);


        [Fact]
        public Task Factory_Method_Name_Using_InterpolatedString_Produces_Expected()
         => CaptureAsync(
             notes: ["The Factory should have a method called `Person`"],
             verifySource: ["World.Factory"],
             source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                namespace World
                {
                    public partial class Factory 
                    {}

                    [AutoFactory(typeof(Factory), $"{nameof(Person)}")]
                    public class Person 
                    {
                        public Person([FromFactory] IEqualityComparer<string?> comparer)
                        {}
                    }
                }
                """]);
    }
}
