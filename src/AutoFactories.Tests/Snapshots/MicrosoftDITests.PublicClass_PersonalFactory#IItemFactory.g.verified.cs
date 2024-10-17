﻿//HintName: IItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

    public interface IItemFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        global::Item Create(global::System.String name);
    }
