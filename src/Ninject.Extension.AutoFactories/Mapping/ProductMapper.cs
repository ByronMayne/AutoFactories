using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;
using System.Diagnostics.CodeAnalysis;

namespace Ninject.AutoFactories.Mapping
{
    internal class ProductMapper : IMapper<ClassDeclarationSyntax, ProductModel>
    {
        private readonly SemanticModel m_semanticModel;
        private readonly ClassAttributeSettings m_classAttributeSettings;
        private readonly IMapper<ConstructorDeclarationSyntax, ConstructorModel> m_constructureMapper;

        public ProductMapper(SemanticModel semanticModel)
        {
            m_semanticModel = semanticModel;
            m_classAttributeSettings = new ClassAttributeSettings();
            m_constructureMapper = new ConstructorMapper(semanticModel);
        }

        public void Map(ClassDeclarationSyntax classDeclaration, ProductModel destination)
        {
            AttributeSyntax classAttribute = classDeclaration
              .AttributeLists
              .SelectMany(a => a.Attributes)
              .Where(a => m_classAttributeSettings.Type.IsEqualavanetTypeName(a.Name.ToString()))
              .First();

            AccessModifier factoryAccessModifier = AccessModifier.Internal;
            string methodName = "Create";
            string @namespace = SyntaxHelpers.GetNamespace(classDeclaration);
            string fullyQualifedFactoryName = string.IsNullOrWhiteSpace(@namespace)
                ? $"{classDeclaration.Identifier.Text}Factory"
                : $"{@namespace}.{classDeclaration.Identifier.Text}Factory";


            if (TryGetFactoryNameArgument(classAttribute, out AttributeArgumentSyntax? factoryNameArgument))
            {
                switch (factoryNameArgument.GetValue(m_semanticModel))
                {
                    case string asString:
                        fullyQualifedFactoryName = asString;
                        break;
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
                }
            }

            if (TryGetMethodNameArgument(classAttribute, out AttributeArgumentSyntax? methodNameArgument))
            {
                // This will only ever be a string 
                if (methodNameArgument.TryGetStringValue(m_semanticModel, out string? parsedValue))
                {
                    methodName = parsedValue;
                }
            }

            destination.FactoryType = new MetadataTypeName(fullyQualifedFactoryName);

            IEnumerable<ConstructorDeclarationSyntax> constructors = classDeclaration.DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                // Fix for: https://github.com/ByronMayne/Ninject.Extensions.AutoFactories/issues/21
                .Where(c => !c.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.StaticKeyword)))
                .ToList();


            destination.FactoryInterfaceType = new MetadataTypeName($"{destination.FactoryType.Namespace}.I{destination.FactoryType.TypeName}");
            destination.ProductType = new MetadataTypeName($"{@namespace}.{classDeclaration.Identifier.Text}");
            destination.Constructors = m_constructureMapper.MapList(constructors);

            if (destination.Constructors.Count == 0)
            {
                destination.Constructors.Add(new ConstructorModel());
            }

            foreach (ConstructorModel method in destination.Constructors)
            {
                method.Name = methodName;
            }
        }

        /// <summary>
        /// Attempts to get the name of the factory from the positional argument
        /// </summary>
        private bool TryGetFactoryNameArgument(AttributeSyntax attributeSyntax, [NotNullWhen(true)] out AttributeArgumentSyntax? result)
        {
            if (attributeSyntax.TryPositionalArgument(0, out result))
            {
                return true;
            }


            if (attributeSyntax.TryGetNameColonArgument(m_classAttributeSettings.FactoryStringArgumentName, out AttributeArgumentSyntax? colofFactoryStringArgument))
            {
                result = colofFactoryStringArgument;
            }

            if (attributeSyntax.TryGetNameColonArgument(m_classAttributeSettings.FactoryTypeArgumentName, out AttributeArgumentSyntax? colonFactoryTypeArgument))
            {
                result = colonFactoryTypeArgument;
            }

            return false;
        }

        /// <summary>
        /// Attempts to get the consturctor positional argument or the named argument for the method named
        /// </summary>
        /// <param name="attributeSyntax">The attribute to attempt to get the value from</param>
        /// <returns>The argument syntax if it's found otherwise null</returns>
        private bool TryGetMethodNameArgument(AttributeSyntax attributeSyntax, [NotNullWhen(true)] out AttributeArgumentSyntax? result)
        {
            result = null;

            if (attributeSyntax.TryPositionalArgument(1, out AttributeArgumentSyntax? postionalArgument))
            {
                result = postionalArgument;
            }

            if (attributeSyntax.TryGetNameColonArgument(m_classAttributeSettings.MethodNameArgumentName, out AttributeArgumentSyntax? colonArgument))
            {
                result = colonArgument;
            }

            if (attributeSyntax.TryGetNamedArgument(m_classAttributeSettings.MethodNamePropertyName, out AttributeArgumentSyntax? namedArgument))
            {
                result = namedArgument;
            }
            return result is not null;
        }
    }
}
