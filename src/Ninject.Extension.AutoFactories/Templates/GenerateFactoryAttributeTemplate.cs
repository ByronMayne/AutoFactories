using Ninject.Extension.AutoFactories;

namespace Ninject.AutoFactory.Templates
{
    internal class GenerateFactoryAttributeTemplate : Template
    {
        public GenerateFactoryAttributeTemplate() : base("Ninject.GenerateFactoryAttribute.g.cs")
        {}

        /// <inheritdoc cref="Template"/>
        protected override string Render()
        {
            return $$"""
                #nullable enable
                using System;

                namespace {{GeneratorSettings.ClassAttribute.Namespace}}
                {
                    /// <summary>
                    /// Applies to a class to have a factory generated for it for each
                    /// constructor. Parameters within the constructor can be marked with 
                    /// <see cref="FromFactoryAttribute"/> to apply they are provided by 
                    /// dependency injection.
                    /// </summary>
                    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
                    internal sealed class {{GeneratorSettings.ClassAttribute.TypeName}} : Attribute
                    {
                        /// <summary>
                        /// Gets the name of the method the methods that will be generated for this class. Note all the methods
                        /// will be the same name except have different parameters. The default value is 'Create'.
                        /// </summary>
                        public string? MethodName { get; set; }

                        /// <summary>
                        /// The fully qualifed name of the factory that will be generated. The default value will match
                        /// the name of the class this attribute is applied on but post-fixed with 'Factory'. Using this attribute
                        /// you can have several class factory methods be contained within one.
                        /// </summary>
                        public string? FactoryFullyQualifiedName { get; set; }
                    }
                }
                """;
        }
    }
}
