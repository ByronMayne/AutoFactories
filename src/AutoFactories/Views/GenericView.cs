using AutoFactories.Types;
using Ninject.AutoFactories;
using System;

namespace AutoFactories.Views
{
    internal class GenericView : View
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
        public override string HintName => $"{Type.QualifiedName}.g.cs";

        public GenericView(string resourceName, Options options) : base(resourceName, options)
        {
            AccessModifier = AccessModifier.Public;
        }

        public static void AddSource(AddSourceDelegate addSource, string resourceName, Options options, Action<GenericView> configure)
        {
            if (addSource is null) throw new ArgumentNullException(nameof(addSource));
            if (resourceName is null) throw new ArgumentNullException(nameof(resourceName));
            if (configure is null) throw new ArgumentNullException(nameof(configure));

            GenericView classView = new GenericView(resourceName, options);
            configure(classView);
            classView.AddSource(addSource);
        }
    }
}
