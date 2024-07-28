using AutoFactories.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System.Collections.Generic;

namespace AutoFactories.Visitors
{
    internal class ClassDeclartionVisitor
    {
        private readonly List<ConstructorDeclarationVisitor> m_constructors;
        private readonly bool m_isAnalyzer;
        private readonly Options m_options;
        private readonly SemanticModel m_semanticModel;

        public bool HasMarkerAttribute { get; private set; }
        public MetadataTypeName Type { get; private set; }
        public AccessModifier AccessModifier { get; private set; }
        public AccessModifier InterfaceAccessModifier { get; private set; }

        public MetadataTypeName FactoryType { get; private set; }

        public IReadOnlyList<ConstructorDeclarationVisitor> Constructors => m_constructors;

        public ClassDeclartionVisitor(
            bool isAnalyzer,
            Options generatorOptions,
            SemanticModel semanticModel)
        {
            m_constructors = [];
            m_options = generatorOptions;
            m_isAnalyzer = isAnalyzer;
            m_semanticModel = semanticModel;
        }

        public void VisitClassDeclaration(ClassDeclarationSyntax classDeclaration)
        {
            if (m_semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            Type = new MetadataTypeName(typeSymbol.ToDisplayString());
            AccessModifier = AccessModifier.FromSymbol(typeSymbol);
            FactoryType = new MetadataTypeName($"{Type}FactoryView");

            foreach (AttributeListSyntax attributeList in classDeclaration.AttributeLists)
            {
                VisitAttributeList(attributeList);
            }

            if (!HasMarkerAttribute && !m_isAnalyzer)
            {
                // Analyzers will keep scanning for errors 
                return;
            }

            foreach (SyntaxNode childNode in classDeclaration.ChildNodes())
            {
                switch (childNode)
                {
                    case ConstructorDeclarationSyntax constructor:
                        VisitConstructorDeclaration(constructor);
                        break;
                }
            }
        }

        private void VisitAttributeList(AttributeListSyntax node)
        {
            foreach (AttributeSyntax attributeSyntax in node.Attributes)
            {
                if (m_semanticModel.GetTypeInfo(attributeSyntax).Type is not ITypeSymbol typeSymbol)
                {
                    continue;
                }

                string displayString = typeSymbol.ToDisplayString();

                if (string.Equals(m_options.ClassAttributeType.QualifiedName, displayString))
                {
                    HasMarkerAttribute = true;
                    return;
                }
            }
        }


        private void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            ConstructorDeclarationVisitor visitor = new(m_isAnalyzer, this, m_options, Type, m_semanticModel);
            visitor.VisitConstructorDeclaration(node);
            m_constructors.Add(visitor);
        }
    }
}
