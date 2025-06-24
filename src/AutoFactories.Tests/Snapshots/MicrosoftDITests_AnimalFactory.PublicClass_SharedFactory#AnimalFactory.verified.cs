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
using Microsoft.Extensions.DependencyInjection;

    internal partial class AnimalFactory : IAnimalFactory
    {
        private readonly global::System.IServiceProvider __serviceProvider;

        public AnimalFactory(global::System.IServiceProvider serviceProvider)
        {
            __serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new instance of  <see cref="Cat"/>
        /// </summary>
        public global::Cat Create()
        {
            return new Cat(
            );
        }


        /// <summary>
        /// Creates a new instance of  <see cref="Dog"/>
        /// </summary>
        public global::Dog Create(string dogName)
        {
            return new Dog(
                dogName            );
        }
    }