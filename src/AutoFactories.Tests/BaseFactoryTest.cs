using Microsoft.CodeAnalysis;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public abstract class BaseFactoryTest : SnapshotTest
    {
        public BaseFactoryTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddGenerator<AutoFactoriesGeneratorHoist>();
        }

        [Fact]
        public Task Class_ExposeAs_ShowsInterface()
            => CaptureAsync(
                notes: ["Factory should be public because the interface is public"],
                verifySource: ["World.PersonFactory"],
                source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                namespace World
                {
                    public interface IPerson { get; }
                
                    [AutoFactory(ExposeAs=typeof(IPerson))]
                    internal class Person : IPerson {}
                }
                """]);

        [Fact]
        public Task Evaluate_NoParameterInstance()
        => CaptureAsync(source: ["""
            using AutoFactories;
            using System.Collections.Generic;

            [AutoFactory]
            public class ReturnResult 
            {
                public int Execute()
                {
                    return 200;
                }
            }
            """,
            """
            public static class Program
            {
                public static int Main(string[] args)
                {
                    IReturnResultFactory factory = new ReturnResultFactory();
                    ReturnResult result = factory.Create();
                    return result.Execute();
                }
            }
            """]);

        [Fact]
        public Task PublicClass_PersonalFactory()
            => CaptureAsync(
                notes: ["'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well"],
                verifySource: ["ItemFactory", "IItemFactory"],
                source:["""
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
                """]);

        [Fact]
        public Task InternalClass_PersonalFactory()
            => CaptureAsync(
                notes: ["'Item' is internal so 'ItemFactory' and 'IItemFactory' must also be internal."],
                verifySource: ["ItemFactory", "IItemFactory"],
                source: ["""
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
                """]);

        [Fact]
        public Task PublicClass_SharedFactory()
            => CaptureAsync(
                notes: ["Both 'Cat' and 'Dog' should be defined within AnimalFactory", "Factory should be internal"],
                verifySource: ["AnimalFactory", "IAnimalFactory"],
                source: ["""
                using AutoFactories;
                using System.Collections.Generic;

                internal partial class AnimalFactory 
                {}
                """, """
                using AutoFactories;

                [AutoFactory(typeof(AnimalFactory))]
                internal class Cat
                {}
                """, """
                using AutoFactories;

                [AutoFactory(typeof(AnimalFactory))]
                internal class Dog
                {
                    public Dog(string dogName)
                    {}
                }
                """]);
    }
}
