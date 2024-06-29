using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;

namespace Ninject.AutoFactories.Mapping
{
    internal class ProductMapper : IMapper<(SemanticModel, ClassDeclarationSyntax), ProductModel>
    {
        private readonly IMapper<ConstructorDeclarationSyntax, ConstructorModel> m_constructureMapper;


        public ProductMapper()
        {
            m_constructureMapper = new ConstructorMapper();
        }

        public void Map((SemanticModel, ClassDeclarationSyntax) source, ProductModel destination)
        {
            ClassDeclarationSyntax classDeclaration = source.Item2;
            SemanticModel semanticModel = source.Item1;

            AttributeSyntax classAttribute = classDeclaration
              .AttributeLists
              .SelectMany(a => a.Attributes)
              .Where(a => GeneratorSettings.ClassAttribute.IsEqualavanetTypeName(a.Name.ToString()))
              .First();


            string factoryTypeName = classAttribute.GetArgumentValue("FactoryFullyQualifiedName", semanticModel, $"{classDeclaration.Identifier.Text}Factory");
            string methodName = classAttribute.GetArgumentValue("MethodName", semanticModel, "Create")!;
            string @namespace = SyntaxHelpers.GetNamespace(classDeclaration);

            destination.ClassAccessModifier = AccessModifier.Internal;
            destination.InterfaceAccessModifier = AccessModifier.Public;
            destination.FactoryType = new MetadataTypeName($"{@namespace}.{factoryTypeName}");
            destination.FactoryInterfaceType = new MetadataTypeName($"{@namespace}.I{factoryTypeName}");
            destination.ProductType = new MetadataTypeName($"{@namespace}.{classDeclaration.Identifier.Text}");
            destination.Constructors = m_constructureMapper.MapList(classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>());

            foreach (ConstructorModel method in destination.Constructors)
            {
                method.Name = methodName;
            }
        }

    }
}
