//HintName: AnimalFactory.g.cs
// -----------------------------| Notes |-----------------------------
// 1. Both 'Cat' and 'Dog' should be defined within AnimalFactory
// 2. Factory should be internal
// -------------------------------------------------------------------
#nullable enable
using System;
using System.Linq;
using Ninject;

    internal partial class AnimalFactoryFactory : IAnimalFactoryFactory
    {
        private readonly global::Ninject.Syntax.IResolutionRoot __resolutionRoot;

        public AnimalFactoryFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
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

            global::Ninject.Activation.IRequest request = __resolutionRoot.CreateRequest(typeof(Cat), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> results = __resolutionRoot.Resolve(request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Cat>(results));
        }


        /// <summary>
        /// Creates a new instance of  <see cref="Dog"/>
        /// </summary>
        public global::Dog Create(global::System.String dogName)
        {
            global::Ninject.Parameters.IParameter[] __parameters = new global::Ninject.Parameters.IParameter[] {
              new global::Ninject.Parameters.ConstructorArgument("dogName", "dogName"),
            };

            global::Ninject.Activation.IRequest request = __resolutionRoot.CreateRequest(typeof(Dog), null, __parameters, isOptional: false, isUnique: true);
            global::System.Collections.Generic.IEnumerable<object> results = __resolutionRoot.Resolve(request);
            return System.Linq.Enumerable.Single(System.Linq.Enumerable.Cast<Dog>(results));
        }
    }
