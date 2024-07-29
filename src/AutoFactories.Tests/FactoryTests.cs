using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class FactoryTests : AbstractTest
    {
        public FactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {}

        public Task PublicClass_PersonalFactory()
            => Compose("""
                using AutoFactories;
                using System.Collections.Generic'

                [AutoFactory]
                public class Human
                {
                    public string Name { get; }

                    public Human(string name, [FromFactory] IEqualityComparer<string?> comparer)
                    {}
                }
                """);

        public Task InternalClass_PersonalFactory()
            => Compose("""
                using AutoFactories;
                using System.Collections.Generic'

                [AutoFactory]
                internal class Human
                {
                    public string Name { get; }

                    public Human(string name, [FromFactory] IEqualityComparer<string?> comparer)
                    {}
                }
                """);

        public Task PublicClass_SharedFactory()
            => Compose("""
                using AutoFactories;
                using System.Collections.Generic'

                public partial class HumanFactory 
                {}

                [AutoFactory(typeof(HumanFactory)]
                internal class Human
                {
                    public string Name { get; }

                    public Human(string name, [FromFactory] IEqualityComparer<string?> comparer)
                    {}
                }
                """);
    }
}
