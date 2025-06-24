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
    internal class ConstructorDeclarationVisitor : SyntaxVisitor<ConstructorDeclarationSyntax>
    {
        private readonly bool m_isAnalyzer;
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
        /// Gets the type that is going to be created
        /// </summary>
        public MetadataTypeName Type { get; }

        /// <summary>
        /// Gets the type that the constructor will make 
        /// </summary>
        public MetadataTypeName ReturnType { get; }

        /// <summary>
        /// Gets the access modifier that the constructor will need to have
        /// </summary>
        public AccessModifier Accessibility { get; private set; }

        /// <summary>
        /// Gets the collection of parameters for the constructor
        /// </summary>
        public IReadOnlyList<ParameterSyntaxVisitor> Parameters => m_parameters;


        /// <summary>
        /// Gets the class tha the constructor is defined within
        /// </summary>
        public ClassDeclarationVisitor Class { get; }

        public ConstructorDeclarationVisitor(
            bool isAnalyzer, 
            ClassDeclarationVisitor classVisitor, 
            INamedTypeSymbol type,
            INamedTypeSymbol returnType, 
            SemanticModel semanticModel)
        {
            m_isAnalyzer = isAnalyzer;
            m_semanticModel = semanticModel;
            m_parameters = new List<ParameterSyntaxVisitor>();
            Type = new MetadataTypeName(type);
            ReturnType = new MetadataTypeName(returnType);
            Accessibility = AccessModifier.FromSymbol(returnType); // Default access is the return type
            Class = classVisitor;
        }

        protected override void Visit(ConstructorDeclarationSyntax syntax)
        {
            IsStatic = syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
            IsPublic = syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

            if ((IsStatic | !IsPublic) && !m_isAnalyzer)
            {
                return;
            }

            VisitParameters(syntax.ParameterList);

            // Returns back the most restrictive permissions 
            // for all the parameters an return type. This should be public or internal
            Accessibility = AccessModifier.MostRestrictive([
                 Accessibility,
                 ..m_parameters.Select(p => p.Accessibility)]);

        }

        private void VisitParameters(ParameterListSyntax parametersList)
        {
            foreach (ParameterSyntax parameter in parametersList.Parameters)
            {
                ParameterSyntaxVisitor visitor = new ParameterSyntaxVisitor(this, m_semanticModel);
                visitor.Accept(parameter);
                m_parameters.Add(visitor);

                AddChild(visitor);
            }
        }
    }
}
