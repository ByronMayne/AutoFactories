namespace Ninject.AutoFactories.Templates
{
    internal class GenerateFactoryAttributeTemplate : Template
    {
        public GenerateFactoryAttributeTemplate() : base("Ninject.GenerateFactoryAttribute.g.cs")
        { }

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
                        public string? FactoryFullyQualifiedName { get; }

                        /// <summary>
                        /// Marks a class to have a unique factory generated for the type.
                        /// </summary>
                        public {{GeneratorSettings.ClassAttribute.TypeName}}()
                        {
                            MethodName = null;
                            FactoryFullyQualifiedName = null;
                        }

                        /// <summary>
                        /// Marks a class to have a factory generated for it. Using this versions allows you to explictly define
                        /// the name of the factory and what the methods should be called.
                        /// </summary>
                        /// <param name="factoryType">The type of the factory you want to add the builder methods to.</param>
                        /// <param name="methodName">The name of the method that will be used to create your instances. When using a common factory you will need to give each type a custom 
                        /// name otherwise the 'CS0111' error saying a member already exists with the given name. Example 'CreateDog', 'CreateHouse' etc</param>
                        public {{GeneratorSettings.ClassAttribute.TypeName}}(Type factoryType, string methodName)
                        {
                            MethodName = methodName;
                            FactoryFullyQualifiedName = factoryType.FullName;
                        }

                        /// <summary>
                        /// Marks a class to have a factory generated for it. Using this versions allows you to explictly define
                        /// the name of the factory and what the methods should be called.
                        /// </summary>
                        /// <param name="factoryType">The fully qualifed name of the factory that should have methods added to it. For example 'MyNamespace.Factories.ObjectFactory'</param>
                        /// <param name="methodName">The name of the method that will be used to create your instances. When using a common factory you will need to give each type a custom 
                        /// name otherwise the 'CS0111' error saying a member already exists with the given name. Example 'CreateDog', 'CreateHouse' etc</param>
                        public {{GeneratorSettings.ClassAttribute.TypeName}}(string factoryFullyQualifiedName, string methodName)
                        {
                            MethodName = methodName;
                            FactoryFullyQualifiedName = factoryFullyQualifiedName;
                        }
                    }
                }
                """;
        }
    }
}
