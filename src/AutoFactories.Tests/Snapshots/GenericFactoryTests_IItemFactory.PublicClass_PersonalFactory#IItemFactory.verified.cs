// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;

    public interface IItemFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        global::Item Create(global::System.String name);
    }