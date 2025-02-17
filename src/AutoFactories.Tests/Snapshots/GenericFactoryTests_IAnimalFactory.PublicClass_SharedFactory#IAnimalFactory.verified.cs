﻿// -----------------------------| Notes |-----------------------------
// 1. Both 'Cat' and 'Dog' should be defined within AnimalFactory
// 2. Factory should be internal
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


    internal interface IAnimalFactory
    {
        /// <summary>
        /// Creates a new instance of  <see cref="Cat"/>
        /// </summary>
        global::Cat Create();

        /// <summary>
        /// Creates a new instance of  <see cref="Dog"/>
        /// </summary>
        global::Dog Create(global::System.String dogName);
    }