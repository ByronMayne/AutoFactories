using Ninject.Extension.AutoFactories;
using Ninject.Extension.AutoFactories.Models;
using System.Diagnostics;

namespace Ninject.AutoFactory.Models
{
    [DebuggerDisplay("{TypeName,nq}: {Constructors.Count}")]
    internal class ProductModel
    {
        public MetadataTypeName ProductType { get; set; }
        public MetadataTypeName FactoryType { get; set; }
        public MetadataTypeName FactoryInterfaceType;
        public AccessModifier InterfaceAccessModifier { get; set; }
        public AccessModifier ClassAccessModifier { get; set; }
        public List<ConstructorModel> Constructors { get; set; }

        public ProductModel()
        {
            Constructors = new List<ConstructorModel>();
            InterfaceAccessModifier = AccessModifier.Public;
            ClassAccessModifier = AccessModifier.Public;
        }
    }
}
