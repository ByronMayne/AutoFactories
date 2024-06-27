using Ninject.Extension.AutoFactories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.AutoFactory.Templates
{
    internal class FromFactoryAttributeTemplate : Template
    {
        public FromFactoryAttributeTemplate() : base("Ninject.FromFactoryAttribute.g.cs")
        {}

        /// <inheritdoc cref="Template"/>
        protected override string Render()
        {
            return $$"""
                #nullable enable
                using System;

                namespace {{GeneratorSettings.ParameterAttribute.Namespace}}
                {
                    /// <summary>
                    /// Applied to a parameter of a constructor or method to impley the factory
                    /// should provide this parameter rathen then the user.
                    /// </summary>
                    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)] 
                    internal sealed class {{GeneratorSettings.ParameterAttribute.TypeName}} : Attribute
                    {
                        public FromFactoryAttribute()
                        {}
                    }
                }
                """;
        }
    }
}
