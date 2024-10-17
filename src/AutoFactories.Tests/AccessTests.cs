using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class AccessTests : BaseFactoryTest
    {
        public AccessTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }


        public void PublicType_NoPartialFactory_Produces_PublicFactoryAndInterface()
        {

        }

        public void InternalType_NoFactory_ProducesInternalFactoryAndInterface()
        {

        }

        public void InternalType_InternalPartialFactory_ProducesInternalFactoryAndInterface()
        {

        }


        public void InternalType_PublicFactory_ProducesPublicFactoryAndNoInterface()
        {

        }
    }
}
