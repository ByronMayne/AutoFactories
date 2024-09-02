using AutoFactories.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoFactories.Visitors
{
    internal class ParameterSyntaxVisitor
    {
        private readonly bool m_isAnalyzer;
        private readonly Options m_options;
        private readonly SemanticModel m_semanticModel;

        public string? Name { get; private set; }
        public MetadataTypeName Type { get; private set; }
        public bool HasMarkerAttribute { get; private set; }

        public Location? AttributeLocation { get; private set; }

        public ConstructorDeclarationVisitor Constructor { get; }

        public ParameterSyntaxVisitor(
            bool isAnalyzer, 
            ConstructorDeclarationVisitor constructor,
            Options options, 
            SemanticModel semanticModel)
        {
            m_isAnalyzer = isAnalyzer;
            m_options = options;
            m_semanticModel = semanticModel;

            Constructor = constructor;
        }

        public void VisitParameter(ParameterSyntax parameter)
        {

            if (parameter.Type is null || m_semanticModel.GetSymbolInfo(parameter.Type).Symbol is not ISymbol typeSymbol)
            {
                return;
            }

            AttributeSyntax? markerAttribute = GetMarkerAttribute(parameter);
            Name = parameter.Identifier.Text;
            HasMarkerAttribute = markerAttribute is not null;
            AttributeLocation = markerAttribute?.GetLocation();
            Type = new MetadataTypeName(typeSymbol.ToDisplayString());
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

                    if (string.Equals(m_options.ParameterAttributeType.QualifiedName, displayString))
                    {
                        return attribute;
                    }
                }
            }
            return null;
        }
    }
}