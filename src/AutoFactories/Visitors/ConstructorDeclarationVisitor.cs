using AutoFactories.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFactories.Visitors
{
    internal class ConstructorDeclarationVisitor
    {
        private readonly bool m_isAnalyzer;
        private readonly Options m_options;
        private readonly SemanticModel m_semanticModel;
        private readonly List<ParameterSyntaxVisitor> m_parameters;

        /// <summary>
        /// Gets if it's a static constructor or not 
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets if the constructor is public or not
        /// </summary>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Gets the type that the constructor will make 
        /// </summary>
        public MetadataTypeName ReturnType { get; }

        /// <summary>
        /// Gets the access modifier that the constructor will need to have
        /// </summary>
        public AccessModifier AccessModifier { get; }

        /// <summary>
        /// Gets the collection of parameters for the constructor
        /// </summary>
        public IReadOnlyList<ParameterSyntaxVisitor> Parameters => m_parameters;


        /// <summary>
        /// Gets the class tha the constructor is defined within
        /// </summary>
        public ClassDeclartionVisitor Class { get; }

        public ConstructorDeclarationVisitor(bool isAnalyzer, ClassDeclartionVisitor classVistor, Options options, MetadataTypeName type, SemanticModel semanticModel)
        {
            m_isAnalyzer = isAnalyzer;
            m_options = options;
            m_semanticModel = semanticModel;
            m_parameters = new List<ParameterSyntaxVisitor>();
            ReturnType = type;
            Class = classVistor;
        }

        public void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            IsStatic = node.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
            IsPublic = node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

            if ((IsStatic | !IsPublic) && !m_isAnalyzer)
            {
                return;
            }

            VisitParameters(node.ParameterList);
        }

        private void VisitParameters(ParameterListSyntax parametersList)
        {
            foreach (ParameterSyntax parameter in parametersList.Parameters)
            {
                ParameterSyntaxVisitor vistor = new ParameterSyntaxVisitor(m_isAnalyzer, this, m_options, m_semanticModel);
                vistor.VisitParameter(parameter);
                m_parameters.Add(vistor);
            }
        }
    }
}
