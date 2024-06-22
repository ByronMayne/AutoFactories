using System.Diagnostics;

namespace Ninject.AutoFactory.Models
{
    [DebuggerDisplay("{TypeName,nq}: {Methods.Count}")]
    internal class FactoryModel
    {
        public string Namespace { get; set; } = "";
        public string TypeName { get; set; } = "";
        public string FactoryTypeName { get; set; } = "";
        public string FactoryInterfaceTypeName { get; set; } = "";
        public AccessModifier InterfaceAccessModifier { get; set; }
        public AccessModifier ClassAccessModifier { get; set; }
        public List<MethodModel> Methods { get; set; }

        public FactoryModel()
        {
            Methods = new List<MethodModel>();
            InterfaceAccessModifier = AccessModifier.Public;
            ClassAccessModifier = AccessModifier.Public;
        }
    }
}
