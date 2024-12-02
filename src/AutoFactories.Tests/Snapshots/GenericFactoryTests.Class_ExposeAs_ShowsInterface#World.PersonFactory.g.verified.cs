//HintName: World.PersonFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. Factory should be public because the interface is public
// -------------------------------------------------------------------
#nullable enable
using System;

namespace World
{
    internal partial class PersonFactory : IPersonFactory
    {
        public PersonFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="World.IPerson"/>
        /// </summary>
        public global::World.IPerson Create()
        {
            global::World.IPerson __result = new global::World.IPerson();
            return __result;
        }
    }
}
