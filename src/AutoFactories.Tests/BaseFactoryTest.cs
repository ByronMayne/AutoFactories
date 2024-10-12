using Microsoft.CodeAnalysis;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public abstract class BaseFactoryTest : AbstractTest
    {
        public BaseFactoryTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {}

        [Fact]
        public Task PublicClass_PersonalFactory()
            => Compose("""
                using AutoFactories;
                using System.Collections.Generic;

                [AutoFactory]
                public class Item
                {
                    public string Name { get; }

                    public Item(string name, [FromFactory] IEqualityComparer<string?> comparer)
                    {
                        Name = name;
                    }
                }
                """,
                notes: ["'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well"],
                verifySource: ["ItemFactory.g.cs", "IItemFactory.g.cs"]);

        [Fact]
        public Task InternalClass_PersonalFactory()
            => Compose("""
                using AutoFactories;
                using System.Collections.Generic;

                [AutoFactory]
                internal class Item
                {
                    public string Name { get; }

                    public Item(string name, [FromFactory] IEqualityComparer<string?> comparer)
                    {
                        Name = name;
                    }
                }
                """,
                notes: ["'Item' is internal so 'ItemFactory' and 'IItemFactory' must also be internal."],
                verifySource: ["ItemFactory.g.cs", "IItemFactory.g.cs"]);

        [Fact]
        public Task PublicClass_SharedFactory()
            => Compose("""
                using AutoFactories;
                using System.Collections.Generic;

                public partial class AnimalFactory 
                {}

                [AutoFactory(typeof(AnimalFactory))]
                internal class Cat
                {}

                [AutoFactory(typeof(AnimalFactory))]
                internal class Dog
                {
                    public Dog(string dogName)
                    {}
                }


                """,
                notes: ["Both 'Cat' and 'Dog' should be defined within AnimalFactory", "Factory should be internal"],
                verifySource: ["AnimalFactory.g.cs", "IAnimalFactory.g.cs"]);
    }
}
