// -----------------------------| Notes |-----------------------------
// 1. The namespace 'using Debugger = System.Diagnostics.Debugger' should be included
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;
using Debugger = System.Diagnostics.Debugger;


    public partial class HumanFactory : IHumanFactory
    {
        public HumanFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Human"/>
        /// </summary>
        public global::Human Create()
        {
            global::Human __result = new global::Human();
            return __result;
        }
    }