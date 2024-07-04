using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Ninject.AutoFactories
{
    public class AutoFactoriesModuleTests : SnapshotTest
    {
        public AutoFactoriesModuleTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddTestSubject("Ninject.FactoryModule.g.cs");
        }

        /// <summary>
        /// Should contain a niject module that has no registrations 
        /// </summary>
        [Fact]
        public Task NoFactoriesSnapshot()
            => Compose();
    }
}
