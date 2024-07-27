using SGF;
using System;

namespace AutoFactories
{
    [SgfGenerator]
    public class AutoFactoriesGenerator : IncrementalGenerator
    {
        public AutoFactoriesGenerator() : base($"AutoFactories")
        {
        }

        public override void OnInitialize(SgfInitializationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
