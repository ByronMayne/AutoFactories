﻿using Ninject;
using Ninject.Syntax;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class NinjectFactoryTests : BaseFactoryTest
    {
        public NinjectFactoryTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddViews(ProjectPaths.NinjectDir / "Views");
            AddAssemblyReference<IKernel>();
        }
    }
}
