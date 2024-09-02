//HintName: ItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is internal so 'ItemFactory' and 'IItemFactory' must also be internal.
// -------------------------------------------------------------------
#nullable enable
using System;

    internal partial class ItemFactoryFactory : IItemFactoryFactory
    {
        private readonly global::System.Collections.Generic.IEqualityComparer<string?> m_comparer;
        public ItemFactoryFactory(
            global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            m_comparer = comparer;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        public global::Item Create(global::System.String name, global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            global::Item __result = new global::Item(
             name,
             m_comparer);
            return __result;
        }
    }
