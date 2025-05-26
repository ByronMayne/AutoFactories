// -----------------------------| Notes |-----------------------------
// 1. Factory should be public because the interface is public
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System;
using System.Linq;

namespace World
{
    public partial class PersonFactory : IPersonFactory
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

            global::Ninject.Activation.IRequest __request = __resolutionRoot.CreateRequest(typeof(World.Person), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> __results = __resolutionRoot.Resolve(__request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<World.Person>(__results));

        }
    }
}