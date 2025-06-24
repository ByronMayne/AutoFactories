// -----------------------------| Notes |-----------------------------
// 1. The Factory should have a method called `Person`
// -------------------------------------------------------------------
using AutoFactories;
using System.Collections.Generic;

namespace World
{
    public partial class Factory 
    {}

    [AutoFactory(typeof(Factory), nameof(Person))]
    public class Person 
    {
        public Person([FromFactory] IEqualityComparer<string?> comparer)
        {}
    }
}
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using System.Collections.Generic;
using AutoFactories;


namespace World
{
    public partial class Factory : IFactory
    {
        private readonly global::System.Collections.Generic.IEqualityComparer<string?> m_comparer;

        public Factory(
            global::System.Collections.Generic.IEqualityComparer<string?> comparer)
        {
            m_comparer = comparer;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.Person"/>
        /// </summary>
        public global::World.Person Person()
        {
            global::World.Person __result = new global::World.Person(
             m_comparer);
            return __result;
        }
    }
}