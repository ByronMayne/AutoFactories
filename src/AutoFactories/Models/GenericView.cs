using AutoFactories.Types;
using Ninject.AutoFactories;

namespace AutoFactories.Models
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
    }
}
