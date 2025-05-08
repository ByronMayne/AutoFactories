using AutoFactories.Diagnostics;
using AutoFactories.Tests.Generators;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    /// <summary>
    /// Tests are validate the behaviour when there are multiple source generators in the same project.
    /// </summary>
    public class MultiGeneratorTests : AbstractTest
    {
        public MultiGeneratorTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddGenerator<AutoFactoriesGeneratorHoist>();
        }



    }
}
