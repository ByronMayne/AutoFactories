// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System.Collections.Generic;
using AutoFactories;


    public partial class ItemFactory : IItemFactory
    {
        private readonly global::System.Collections.Generic.IEqualityComparer<string?> m_comparer;

        public ItemFactory(
global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            m_comparer = comparer;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        public global::Item Create(string name)
        {
            global::Item __result = new global::Item(
             name,
             m_comparer);
            return __result;
        }
    }