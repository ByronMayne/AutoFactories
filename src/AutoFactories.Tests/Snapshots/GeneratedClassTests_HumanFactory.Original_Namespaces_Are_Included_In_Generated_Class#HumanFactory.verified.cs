// -----------------------------| Notes |-----------------------------
// 1. The namespace 'System.IO' should be included
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;
using System.IO;
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