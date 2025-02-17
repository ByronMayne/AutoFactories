using Xunit.Abstractions;

namespace AutoFactories.Tests
{
    public class GeneratedClassTests : SnapshotTest
    {
        public GeneratedClassTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            AddGenerator<AutoFactoriesGeneratorHoist>();
        }

        [Fact]
        public Task Shared_Factory_Creates_Interface()
            => CaptureAsync(
                notes: ["The HumanFactory should also generate a shared IHumanFactory"],
                verifySource: ["HumanFactory", "IHumanFactory"],
                source: ["""
                    using AutoFactories;
                    public interface IHuman
                    {}
                    [AutoFactory(FactoryType=typeof(HumanFactory))]
                    public class Human 
                    {}
                    """,
                    """
                    public partial class HumanFactory
                    {}
                    """]);

        [Fact]
        public Task Original_Namespaces_Are_Included_In_Generated_Class()
            => CaptureAsync(
                notes: ["The namespace 'System.IO' should be included"],
                verifySource: ["HumanFactory"],
                source: ["""
                    using System;
                    using System.IO;
                    using AutoFactories;

                    [AutoFactory]
                    public class Human 
                    {}
                    """]);

        [Fact]
        public Task Static_including_Are_Included_In_Generated_Class()
            => CaptureAsync(
                notes: ["The namespace 'using static System.Console' should be included"],
                verifySource: ["HumanFactory"],
                source: ["""
                    using static System.Console;
                    using AutoFactories;

                    [AutoFactory]
                    public class Human 
                    {}
                    """]);

        [Fact]
        public Task Type_Alias_Is_Included_In_Generated_Class()
            => CaptureAsync(
                notes: ["The namespace 'using Debugger = System.Diagnostics.Debugger' should be included"],
                verifySource: ["HumanFactory"],
                source: ["""
                    using Debugger = System.Diagnostics.Debugger;
                    using AutoFactories;

                    [AutoFactory]
                    public class Human 
                    {}
                    """]);

    }
}
