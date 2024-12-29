// -----------------------------| Notes |-----------------------------
// 1. Factory should be public because the interface is public
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace World
{
    internal partial class PersonFactory : IPersonFactory
    {
        private readonly global::System.IServiceProvider __serviceProvider;

        public PersonFactory(global::System.IServiceProvider serviceProvider)
        {
            __serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.IPerson"/>
        /// </summary>
        public global::World.IPerson Create()
        {
            return new World.Person(
            );
        }
    }
}