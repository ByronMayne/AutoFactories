using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Ninject.AutoFactories
{
    public class ConstantAttributes : SourceGeneratorTestBase
    {
        public ConstantAttributes(ITestOutputHelper outputHelper) : base(outputHelper)
        {}

        [Fact]
        public void NoAttributes_GeneratesNoClasses()
            => Compose(
                    filters: [ Filters.Only("Ninject.FactoryModule.g.cs") ]
                );
    }
}
