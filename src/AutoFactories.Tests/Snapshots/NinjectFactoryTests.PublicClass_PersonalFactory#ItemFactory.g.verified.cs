//HintName: ItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is public so 'ItemFactory' and 'IItemFactory' should be public as well
// -------------------------------------------------------------------
#nullable enable
using System;
using System.Linq;
using Ninject;

    public partial class ItemFactoryFactory : IItemFactoryFactory
    {
        private readonly global::Ninject.Syntax.IResolutionRoot __resolutionRoot;

        public ItemFactoryFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            __resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        public global::Item Create(global::System.String name, global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
              new global::Ninject.Parameters.ConstructorArgument("name", "name"),
            };

            global::Ninject.Activation.IRequest request = __resolutionRoot.CreateRequest(typeof(Item), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> results = __resolutionRoot.Resolve(request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Item>(results));
        }
    }
