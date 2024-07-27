using AutoFactories.Types;
using Ninject.AutoFactories;
using System;

namespace AutoFactories.Views
{
    internal class GenericClassView : View
    {
        /// <summary>
        /// Gets or sets the access modifier for the class
        /// </summary>
        public AccessModifier AccessModifier { get; set; }

        /// <summary>
        /// Get or sets the type of the class
        /// </summary>
        public MetadataTypeName Type { get; set; }

        /// <inheritdoc cref="View"/>
        public override string HintName => $"{Type.QualifedName}.g.cs";

        public GenericClassView(string resourceName) : base(resourceName)
        {
            AccessModifier = AccessModifier.Public;
        }

        public static void AddSource(AddSourceDelegate addSource, string resourceName, Action<GenericClassView> configure)
        {
            if (addSource is null) throw new ArgumentNullException(nameof(addSource));
            if (resourceName is null) throw new ArgumentNullException(nameof(resourceName));
            if (configure is null) throw new ArgumentNullException(nameof(configure));

            GenericClassView classView = new GenericClassView(resourceName);
            configure(classView);
            string source = classView.Transform();
            addSource(classView.HintName, source);

        }
    }
}
