using AutoFactories.Diagnostics;
using FluentAssertions;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class DiagnosticsTests : AbstractTest
    {
        public DiagnosticsTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

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
                assertDiagnostics: d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.UnmarkedFactory));

        [Fact]
        public Task PublicFactory_WithInternalClass_EmitsInconsistentFactoryAcessibility()
            => Compose($$"""
                using AutoFactories;

                public class ChairFactory 
                {}

                [AutoFactory(typeof(ChairFactory)]
                internal class Chair 
                {}
                """,
                assertDiagnostics: d => d.Should().OnlyContain(d => d.Id == DiagnosticIdentifier.InconsistentFactoryAcessibility));
    }
}
