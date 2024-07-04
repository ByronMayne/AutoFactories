using Xunit.Abstractions;

namespace Ninject.AutoFactories
{
    public class GenerateFactoryAttributeTests : SnapshotTest
    {
        public GenerateFactoryAttributeTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddTestSubject("World.PersonFactory.g.cs");
            AddTestSubject("World.CustomFactory.g.cs");
            AddTestSubject("REPLACED_Person_FACTORY.g.cs");
        }

        [Fact]
        public Task DefaultConstructor()
            => Compose(
                source: CreateClass());

        [Fact]
        public Task StringFactoryName()
            => Compose(
                source: CreateClass("\"World.CustomFactory\""));

        [Fact]
        public Task StringFactoryCustomMethod()
        => Compose(
           source: CreateClass("\"World.CustomFactory\"", "\"CustomFunction\""));

        [Fact]
        public Task DefaultConstructorMethodEquals()
            => Compose(
                source: CreateClass("MethodName = \"OVERRIDDEN_METHOD\""));

        // Bug: https://github.com/ByronMayne/Ninject.Extensions.AutoFactories/issues/4
        [Fact]
        public Task FactoryNameInterpolatedString()
            => Compose(
                source: CreateClass("$\"REPLACED_{nameof(Person)}_FACTORY\""));

        [Fact]
        public Task MethodNameInterpolatedString()
          => Compose(
                source: CreateClass("MethodName = $\"REPLACED_{nameof(Person)}\""));

        /// <summary>
        /// Validates that using `typeof(CustomFactory)` generates correctly 
        /// </summary>
        [Fact]
        public Task TypeOfFactory()
            => Compose(
                source: CreateClass("typeof(CustomFactory)"));

        private string CreateClass(params string[] attributeArguments)
        {
            return $$"""
                using Ninject;

                namespace World
                {
                    [GenerateFactory({{string.Join(", ", attributeArguments)}})]
                    public class Person
                    {

                    }

                    public partial class CustomFactory
                    {
                    }
                }
                """;
        }
    }
}
