using AutoFactories.Diagnostics;
using FluentAssertions;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class DiagnosticsTests : AbstractTest
    {
        public DiagnosticsTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddAnalyzer<AutoFactoriesAnalyzer>();
            AddGenerator<AutoFactoriesGeneratorHoist>();
        }


        [Fact]
        public Task ExposeAsNotDerived()
            => Compose($$"""
                using AutoFactories;

                [AutoFactory(ExposeAs=typeof(int))]
                public class Human
                {
                }

                """,
                assertAnalyuzerResult: d => d.Should()
                    .OnlyContain(d => d.Id == DiagnosticIdentifier.ExposedAsIsNotDerivedType));


        [Fact]
        public Task ExpsedAsProducesNoError()
            => Compose($$"""
                using AutoFactories;

                public interface IHuman 
                {}

                [AutoFactory(ExposeAs=typeof(IHuman))]
                public class Human : IHuman
                {
                }

                """,
                assertAnalyuzerResult: d => d.Should()
                    .BeEmpty());

        [Fact]
        public Task UntaggedClass()
            => Compose($$"""
                using AutoFactories; 

                public class UntaggedFactory
                {
                    public UntaggedFactory([FromFactory] string name)
                    {}
                }
                """,
                assertAnalyuzerResult: d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.UnmarkedFactory));

        [Fact]
        public Task PublicFactory_WithInternalClass_EmitsInconsistentFactoryAcessibility()
            => Compose($$"""
                using AutoFactories;

                public partial class ChairFactory 
                {}

                [AutoFactory(typeof(ChairFactory))]
                internal class Chair 
                {}
                """,
                assertAnalyuzerResult: d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.InconsistentFactoryAcessibility));
    }
}
