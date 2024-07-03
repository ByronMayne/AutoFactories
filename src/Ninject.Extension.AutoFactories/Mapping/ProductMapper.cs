using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;
using System.Diagnostics;
using System.Linq;

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

            AccessModifier factoryAccessModifier = AccessModifier.Internal;
            string methodName = "Create";
            string @namespace = SyntaxHelpers.GetNamespace(classDeclaration);
            string fullyQualifedFactoryName = string.IsNullOrWhiteSpace(@namespace)
                ? $"{classDeclaration.Identifier.Text}Factory"
                : $"{@namespace}.{classDeclaration.Identifier.Text}Factory";


            if (classAttribute.ArgumentList != null)
            {
                // THe constructor can be one of the following
                // 1. ()
                // 2. (typeof(MyType), "MethodName")
                // 3. ("Namespace.MyType", "MethodName");
                AttributeArgumentSyntax[] positionalArgs = classAttribute.ArgumentList.Arguments.OfType<AttributeArgumentSyntax>().ToArray();
                if (positionalArgs.Length > 0)
                {
                    object? value = positionalArgs[0].GetValue(semanticModel);
                    switch (value)
                    {
                        case INamedTypeSymbol namedTypeSymbol:
                            fullyQualifedFactoryName = namedTypeSymbol.ToDisplayString(NullableFlowState.NotNull);
                            switch (namedTypeSymbol.DeclaredAccessibility)
                            {
                                case Accessibility.Public:
                                    destination.FactoryAccessModifier = AccessModifier.Public;
                                    break;
                                case Accessibility.Internal:
                                    destination.FactoryAccessModifier = AccessModifier.Internal;
                                    break;
                                case Accessibility.Private:
                                    destination.FactoryAccessModifier = AccessModifier.Private;
                                    break;
                                case Accessibility.ProtectedAndInternal:
                                    destination.FactoryAccessModifier = AccessModifier.ProtectedAndInternal;
                                    break;
                            }
                            break;
                        case string asString:
                            fullyQualifedFactoryName = asString;
                            break;
                    }
                }

                if (positionalArgs.Length > 1)
                {
                    if (positionalArgs[1].GetValue(semanticModel) is string asString)
                    {
                        methodName = asString;
                    }
                }

                // We can also define just the MethodName 
                if(classAttribute.GetNamedArgumentValue<string>("MethodName", semanticModel, null) is string stringValue)
                {
                    methodName = stringValue;
                }
            }

            destination.FactoryType = new MetadataTypeName(fullyQualifedFactoryName);


            destination.FactoryInterfaceType = new MetadataTypeName($"{destination.FactoryType.Namespace}.I{destination.FactoryType.TypeName}");
            destination.ProductType = new MetadataTypeName($"{@namespace}.{classDeclaration.Identifier.Text}");
            destination.Constructors = m_constructureMapper.MapList(classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>());

            if (destination.Constructors.Count == 0)
            {
                destination.Constructors.Add(new ConstructorModel());
            }

            foreach (ConstructorModel method in destination.Constructors)
            {
                method.Name = methodName;
            }
        }

    }
}
