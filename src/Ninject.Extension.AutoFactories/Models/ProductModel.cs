using System.Diagnostics;

namespace Ninject.AutoFactories.Models
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
            Constructors = [];
            InterfaceAccessModifier = AccessModifier.Public;
            ClassAccessModifier = AccessModifier.Public;
        }
    }
}
