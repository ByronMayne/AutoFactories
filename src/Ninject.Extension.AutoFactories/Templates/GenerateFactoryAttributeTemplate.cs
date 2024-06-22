namespace Ninject.AutoFactory.Templates
{
    internal class GenerateFactoryAttributeTemplate : Template
    {
        public GenerateFactoryAttributeTemplate() : base("Ninject.GenerateFactoryAttribute.g.cs")
        {}

        /// <inheritdoc cref="Template"/>
        protected override string Render()
        {
            return """
                using System;

                namespace Ninject
                {
                    /// <summary>
                    /// Applies to a class to have a factory generated for it for each
                    /// constructor. Parameters within the constructor can be marked with 
                    /// <see cref="FromFactoryAttribute"/> to apply they are provided by 
                    /// dependency injection.
                    /// </summary>
                    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
                    internal sealed class GenerateFactoryAttribute : Attribute
                    {}
                }
                """;
        }
    }
}
