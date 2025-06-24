using AutoFactories.Diagnostics;
using AutoFactories.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System.Collections;

namespace AutoFactories.Visitors
{

    internal class ParameterSyntaxVisitor : SyntaxVisitor<ParameterSyntax>
    {
        private readonly SemanticModel m_semanticModel;


        public string? Name { get; private set; }
        public MetadataTypeName Type { get; private set; }
        public bool HasMarkerAttribute { get; private set; }

        public AccessModifier Accessibility { get; private set; }

        public Location? AttributeLocation { get; private set; }

        public ConstructorDeclarationVisitor Constructor { get; }


        /// <summary>
        /// When we have a parameter that is generated from another source generator we can't
        /// actually resolve it. We can't get the type name or the fully qualified name. So we have to warn the user 
        /// </summary>
        public bool IsTypeResolved { get; private set; }

        public ParameterSyntaxVisitor(
            ConstructorDeclarationVisitor constructor,
            SemanticModel semanticModel)
        {
            Constructor = constructor;
            m_semanticModel = semanticModel;
        }


        protected override void Visit(ParameterSyntax syntax)
        {
            ITypeSymbol? typeSymbol = null;

            if (syntax.Type is not null)
            {
                typeSymbol = m_semanticModel.GetSymbolInfo(syntax.Type).Symbol as ITypeSymbol;
            }

            AttributeSyntax? markerAttribute = GetMarkerAttribute(syntax);
            Name = syntax.Identifier.Text;
            HasMarkerAttribute = markerAttribute is not null;
            AttributeLocation = markerAttribute?.GetLocation();

            if (typeSymbol is not null)
            {
                IsTypeResolved = true;
                Type = new MetadataTypeName(typeSymbol)
                {
                    IsNullable = syntax.Type is NullableTypeSyntax
                };
                Accessibility = AccessModifier.FromSymbol(typeSymbol);
            }
            else
            {
                IsTypeResolved = false;
                string typeName = $"{syntax.Type}";
                string @namespace = "";
                int splitIndex = typeName.LastIndexOf('.');

                if(splitIndex > 0)
                {
                    typeName = typeName.Substring(splitIndex + 1);
                    @namespace = typeName.Substring(0, splitIndex);
                }
                Type = new MetadataTypeName(typeName, @namespace, false, false);
                Accessibility = AccessModifier.Public; // We don't know what it is 

                AddDiagnostic(UnresolvedParameterTypeDiagnostic.Get(this));
            }

        }

        private AttributeSyntax? GetMarkerAttribute(ParameterSyntax node)
        {
            foreach (AttributeListSyntax attributeList in node.AttributeLists)
            {
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    if (m_semanticModel.GetTypeInfo(attribute).Type is not ITypeSymbol typeSymbol)
                    {
                        continue;
                    }

                    string displayString = typeSymbol.ToDisplayString();

                    if (string.Equals(TypeNames.ParameterAttributeType.QualifiedName, displayString))
                    {
                        return attribute;
                    }
                }
            }
            return null;
        }
    }
}