using Boxed.Mapping;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactory.Models;

namespace Ninject.AutoFactory.Mapping
{
    internal class FactoryMapper : IMapper<ClassDeclarationSyntax, FactoryModel>
    {
        private readonly IMapper<ConstructorDeclarationSyntax, MethodModel> m_methodMapper;

        public FactoryMapper()
        {
            m_methodMapper = new MethodMapper();
        }

        public void Map(ClassDeclarationSyntax source, FactoryModel destination)
        {
            destination.ClassAccessModifier = AccessModifier.Internal;
            destination.InterfaceAccessModifier = AccessModifier.Public;
            destination.Namespace = SyntaxHelpers.GetNamespace(source);
            destination.TypeName = source.Identifier.Text;
            destination.FactoryTypeName = $"{destination.TypeName}Factory";
            destination.FactoryInterfaceTypeName = $"I{destination.TypeName}Factory";
            destination.Methods = m_methodMapper.MapList(source.DescendantNodes().OfType<ConstructorDeclarationSyntax>());
        }
    }
}
