using Xunit.Abstractions;

namespace Ninject.AutoFactories
{
    public class GeneratorTests : SnapshotTest
    {
        public GeneratorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        { }

        /// <summary>
        /// Validates that types with static constructors are generated correctly
        /// <see cref="https://github.com/ByronMayne/Ninject.Extensions.AutoFactories/issues/21"/>
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task StaticConstructor()
        {
            return Compose(
                        testSubject: "Foo.BarFactory.g.cs",
                        source: """
                using Ninject;

                namespace Foo
                {
                    [GenerateFactory]
                    public class Bar
                    {
                        public Bar(string parameter)
                        {}

                        static Bar()
                        {}
                    }
                }
                """);
        }
    }
}
