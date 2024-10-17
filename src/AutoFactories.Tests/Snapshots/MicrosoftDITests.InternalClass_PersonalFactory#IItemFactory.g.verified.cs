﻿//HintName: IItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is internal so 'ItemFactory' and 'IItemFactory' must also be internal.
// -------------------------------------------------------------------
#nullable enable
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

    internal interface IItemFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        global::Item Create(global::System.String name);
    }