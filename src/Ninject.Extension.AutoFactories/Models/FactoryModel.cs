using Ninject.AutoFactory;
using Ninject.AutoFactory.Models;
using System.Diagnostics;

namespace Ninject.Extension.AutoFactories.Models
{
    [DebuggerDisplay("{Type}")]
    internal class FactoryModel
    {
        public MetadataTypeName Type { get; set; }
        public AccessModifier AccessModifier { get; set; }
        public MetadataTypeName InterfaceType { get; set; }
        public AccessModifier InterfaceAccessModifier { get; }
        public List<ProductModel> Products { get; set; }

        public FactoryModel(MetadataTypeName type)
        {
            Type = type;

            string interfaceName = $"I{type.TypeName}";
            if (!string.IsNullOrWhiteSpace(type.Namespace)) interfaceName = $"{type.Namespace}.{interfaceName}";

            InterfaceType = new MetadataTypeName(interfaceName);
            AccessModifier = AccessModifier.Internal;
            InterfaceAccessModifier = AccessModifier.Public;
            Products = new List<ProductModel>();
        }
    }
}
