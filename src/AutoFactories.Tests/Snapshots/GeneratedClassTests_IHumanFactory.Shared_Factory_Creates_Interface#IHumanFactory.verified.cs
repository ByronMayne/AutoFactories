// -----------------------------| Notes |-----------------------------
// 1. The HumanFactory should also generate a shared IHumanFactory
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


    public interface IHumanFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Human"/>
        /// </summary>
        global::Human Create();
    }