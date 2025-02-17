// -----------------------------| Notes |-----------------------------
// 1. The HumanFactory should also generate a shared IHumanFactory
// -------------------------------------------------------------------
public partial class HumanFactory
{}
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


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