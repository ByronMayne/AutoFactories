//HintName: ItemFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. 'Item' is internal so 'ItemFactory' and 'IItemFactory' must also be internal.
// -------------------------------------------------------------------
#nullable enable
using System;
using System.Linq;
using Ninject;

    internal partial class ItemFactoryFactory : IItemFactoryFactory
    {
        private readonly global::System.Collections.Generic.IEqualityComparer<string?> m_comparer;
        private readonly global::Ninject.Syntax.IResolutionRoot m_resolutionRoot;

        public ItemFactoryFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            m_resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Item"/>
        /// </summary>
        public global::Item Create(global::System.String name, global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
              new global::Ninject.Parameters.ConstructorArgument("name", "name"),
            };

            global::Ninject.Activation.IRequest request = m_resolutionRoot.CreateRequest(typeof(Item), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> results = m_resolutionRoot.Resolve(request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Item>(results));
        }
    }
