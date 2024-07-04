using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Ninject.AutoFactories.Mapping
{
    internal class ProductMapper : IMapper<(SemanticModel, ClassDeclarationSyntax), ProductModel>
    {
        private readonly ClassAttributeSettings m_classAttributeSettings;
        private readonly IMapper<ConstructorDeclarationSyntax, ConstructorModel> m_constructureMapper;

        public ProductMapper()
        {
            m_classAttributeSettings = new ClassAttributeSettings();
            m_constructureMapper = new ConstructorMapper();
        }



        public void Map((SemanticModel, ClassDeclarationSyntax) source, ProductModel destination)
        {
            ClassDeclarationSyntax classDeclaration = source.Item2;
            SemanticModel semanticModel = source.Item1;

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
                switch (factoryNameArgument.GetValue(semanticModel))
                {
                    case string asString:
                        fullyQualifedFactoryName = asString;
                        break;
                    case INamedTypeSymbol namedTypeSymbol:
                        fullyQualifedFactoryName = namedTypeSymbol.ToDisplayString(NullableFlowState.NotNull);
                        break;
                }
            }

            if (TryGetMethodNameArgument(classAttribute, out AttributeArgumentSyntax? methodNameArgument))
            {
                // This will only ever be a string 
                if (methodNameArgument.TryGetStringValue(semanticModel, out string? parsedValue))
                {
                    methodName = parsedValue;
                }
            }




            //if (classAttribute.ArgumentList != null)
            //{
            //    // THe constructor can be one of the following
            //    // 1. ()
            //    // 2. (typeof(MyType), "MethodName")
            //    // 3. ("Namespace.MyType", "MethodName");
            //    AttributeArgumentSyntax[] positionalArgs = classAttribute.ArgumentList.Arguments.OfType<AttributeArgumentSyntax>().ToArray();
            //    if (positionalArgs.Length > 0)
            //    {
            //        object? value = positionalArgs[0].GetValue(semanticModel);
            //        switch (value)
            //        {
            //            case INamedTypeSymbol namedTypeSymbol:
            //                fullyQualifedFactoryName = namedTypeSymbol.ToDisplayString(NullableFlowState.NotNull);
            //                switch (namedTypeSymbol.DeclaredAccessibility)
            //                {
            //                    case Accessibility.Public:
            //                        destination.FactoryAccessModifier = AccessModifier.Public;
            //                        break;
            //                    case Accessibility.Internal:
            //                        destination.FactoryAccessModifier = AccessModifier.Internal;
            //                        break;
            //                    case Accessibility.Private:
            //                        destination.FactoryAccessModifier = AccessModifier.Private;
            //                        break;
            //                    case Accessibility.ProtectedAndInternal:
            //                        destination.FactoryAccessModifier = AccessModifier.ProtectedAndInternal;
            //                        break;
            //                }
            //                break;
            //            case string asString:
            //                fullyQualifedFactoryName = asString;
            //                break;
            //        }
            //    }

            //    if (positionalArgs.Length > 1)
            //    {
            //        if (positionalArgs[1].GetValue(semanticModel) is string asString)
            //        {
            //            methodName = asString;
            //        }
            //    }

            //    // We can also define just the MethodName 
            //    if (classAttribute.GetNamedArgumentValue<string>("MethodName", semanticModel, null) is string stringValue)
            //    {
            //        methodName = stringValue;
            //    }
            //}

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

        /// <summary>
        /// Attempts to get the name of the factory from the positional argument
        /// </summary>
        private bool TryGetFactoryNameArgument(AttributeSyntax attributeSyntax, [NotNullWhen(true)] out AttributeArgumentSyntax? result)
        {
            result = null;

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
