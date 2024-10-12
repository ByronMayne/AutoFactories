//HintName: ItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
using System;

    public partial class ItemFactoryFactory : IItemFactoryFactory
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
