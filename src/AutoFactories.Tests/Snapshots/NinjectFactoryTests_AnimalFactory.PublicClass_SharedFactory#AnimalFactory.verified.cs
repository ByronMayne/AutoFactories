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

using System;
using System.Linq;

    internal partial class AnimalFactory : IAnimalFactory
    {
        private readonly global::Ninject.Syntax.IResolutionRoot __resolutionRoot;

        public AnimalFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            __resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Cat"/>
        /// </summary>
        public global::Cat Create()
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
            };

            global::Ninject.Activation.IRequest __request = __resolutionRoot.CreateRequest(typeof(Cat), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> __results = __resolutionRoot.Resolve(__request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Cat>(__results));

        }


        /// <summary>
        /// Creates a new instance of  <see cref="Dog"/>
        /// </summary>
        public global::Dog Create(string dogName)
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
              new global::Ninject.Parameters.ConstructorArgument("dogName", dogName),
            };

            global::Ninject.Activation.IRequest __request = __resolutionRoot.CreateRequest(typeof(Dog), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> __results = __resolutionRoot.Resolve(__request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Dog>(__results));

        }
    }