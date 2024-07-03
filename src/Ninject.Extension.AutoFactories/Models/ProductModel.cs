using System.Diagnostics;

namespace Ninject.AutoFactories.Models
{
    [DebuggerDisplay("{ProductType,nq}: {Constructors.Count}")]
    internal class ProductModel
    {
        public MetadataTypeName ProductType { get; set; }
        public MetadataTypeName FactoryType { get; set; }
        public MetadataTypeName FactoryInterfaceType;
        public AccessModifier? InterfaceAccessModifier { get; set; }
        public AccessModifier? FactoryAccessModifier { get; set; }
        public List<ConstructorModel> Constructors { get; set; }

        public ProductModel()
        {
            Constructors = [];
            InterfaceAccessModifier = null;
            FactoryAccessModifier = null;
        }
    }
}
