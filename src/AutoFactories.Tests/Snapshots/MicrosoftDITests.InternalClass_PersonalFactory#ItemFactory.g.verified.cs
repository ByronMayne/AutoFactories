//HintName: ItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is internal so 'ItemFactory' and 'IItemFactory' must also be internal.
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

    internal partial class ItemFactory : IItemFactory
    {
        private readonly global::System.IServiceProvider __serviceProvider;

        public ItemFactory(global::System.IServiceProvider serviceProvider)
        {
            __serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        public global::Item Create(global::System.String name)
        {
            return new Item(
                name,
                __serviceProvider.GetRequiredService<System.Collections.Generic.IEqualityComparer<string?>>()
            );
        }
    }
