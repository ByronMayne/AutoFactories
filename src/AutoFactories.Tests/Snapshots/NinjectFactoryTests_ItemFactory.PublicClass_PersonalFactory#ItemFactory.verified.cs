// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;
using System.Linq;

    public partial class ItemFactory : IItemFactory
    {
        private readonly global::Ninject.Syntax.IResolutionRoot __resolutionRoot;

        public ItemFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            __resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        public global::Item Create(global::System.String name)
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
              new global::Ninject.Parameters.ConstructorArgument("name", name),
            };

            global::Ninject.Activation.IRequest __request = __resolutionRoot.CreateRequest(typeof(Item), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> __results = __resolutionRoot.Resolve(__request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Item>(__results));

        }
    }