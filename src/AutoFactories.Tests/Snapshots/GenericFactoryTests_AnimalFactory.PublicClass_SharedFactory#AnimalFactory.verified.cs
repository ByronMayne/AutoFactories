// -----------------------------| Notes |-----------------------------
// 1. Both 'Cat' and 'Dog' should be defined within AnimalFactory
// 2. Factory should be internal
// -------------------------------------------------------------------
using AutoFactories;
using System.Collections.Generic;

internal partial class AnimalFactory 
{}
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


    internal partial class AnimalFactory : IAnimalFactory
    {
        public AnimalFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Cat"/>
        /// </summary>
        public global::Cat Create()
        {
            global::Cat __result = new global::Cat();
            return __result;
        }


        /// <summary>
        /// Creates a new instance of  <see cref="Dog"/>
        /// </summary>
        public global::Dog Create(global::System.String dogName)
        {
            global::Dog __result = new global::Dog(
             dogName);
            return __result;
        }
    }