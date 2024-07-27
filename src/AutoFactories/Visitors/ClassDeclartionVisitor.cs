using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFactories.Visitors
{
    internal class ClassDeclartionVisitor
    {
        private readonly bool m_isAnalyzer;
        private readonly SemanticModel m_semanticModel;

        public AccessModifier AccessModifier { get; private set; }
        public AccessModifier InterfaceAccessModifier { get; private set; }


        public ClassDeclartionVisitor(bool isAnalyzer, SemanticModel semanticModel)
        {
            m_isAnalyzer = isAnalyzer;
            m_semanticModel = semanticModel;
        }

        public void VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            foreach (AttributeListSyntax attributeList in classDeclaration.AttributeLists)
            {
                VisitAttributeList(attributeList);
            }
        }

        private void VisitAttributeList(AttributeListSyntax node)
        {
            foreach (AttributeSyntax attributeSyntax in node.Attributes)
            {
                ISymbol? symbol = m_semanticModel.GetDeclaredSymbol(attributeSyntax);
                var typeInfo = m_semanticModel.GetTypeInfo(attributeSyntax);
            }
        }
    }
}
