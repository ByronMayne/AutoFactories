//HintName: IItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
using System;
using System.Linq;
using Ninject;

    public interface IItemFactoryFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        global::Item Create(global::System.String name, global::System.Collections.Generic.IEqualityComparer<string?> comparer);
    }
