//HintName: World.PersonFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. Factory should be public because the interface is public
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;
using System.Linq;

namespace World
{
    internal partial class PersonFactory : IPersonFactory
    {
        private readonly global::Ninject.Syntax.IResolutionRoot __resolutionRoot;

        public PersonFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            __resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.IPerson"/>
        /// </summary>
        public global::World.IPerson Create()
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
            };

            global::Ninject.Activation.IRequest request = __resolutionRoot.CreateRequest(typeof(World.IPerson), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object>
    results = __resolutionRoot.Resolve(request);
    return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<World.IPerson>(results));

        }
    }
}
