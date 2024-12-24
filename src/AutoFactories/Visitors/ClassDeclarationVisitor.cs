using AutoFactories.Diagnostics;
using AutoFactories.Extensions;
using AutoFactories.Types;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace AutoFactories.Visitors
{
    [DebuggerDisplay("{Type,nq}")]
    internal class ClassDeclarationVisitor
    {
        private readonly List<ConstructorDeclarationVisitor> m_constructors;
        private readonly bool m_isAnalyzer;
        private readonly Options m_options;
        private readonly SemanticModel m_semanticModel;

        public bool HasMarkerAttribute { get; private set; }
        public string MethodName { get; private set; }
        public MetadataTypeName Type { get; private set; }
        public AccessModifier AccessModifier { get; private set; }
        public AccessModifier InterfaceAccessModifier { get; private set; }

        public MetadataTypeName FactoryType { get; private set; }

        public Location? MethodNameLocation { get; private set; }
        public Location? FactoryTypeLocation { get; private set; }
        public Location? ExposeAsLocation { get; private set; }

        public AccessModifier FactoryAccessModifier { get; private set; }
        public IReadOnlyList<ConstructorDeclarationVisitor> Constructors => m_constructors;

        /// <summary>
        /// Gets the access modifier that the constructor will need to have
        /// </summary>
        public AccessModifier Accessibility { get; private set; }

        private INamedTypeSymbol? m_typeSymbol;
        private INamedTypeSymbol? m_returnTypeSymbol;

        public ClassDeclarationVisitor(
            bool isAnalyzer,
            Options generatorOptions,
            SemanticModel semanticModel)
        {
            MethodName = "";
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
            m_returnTypeSymbol = typeSymbol; // Default to current type 
            AccessModifier = AccessModifier.FromSymbol(typeSymbol);
            FactoryAccessModifier = AccessModifier;
            FactoryType = new MetadataTypeName($"{Type}Factory");

            m_typeSymbol = typeSymbol;
            m_returnTypeSymbol = typeSymbol;

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

            // Add default 
            if (m_constructors.Count == 0)
            {
                m_constructors.Add(new ConstructorDeclarationVisitor(m_isAnalyzer, this, m_options, m_returnTypeSymbol, m_semanticModel));
            }

            AccessModifier returnTypeAccessibility = AccessModifier.FromSymbol(m_returnTypeSymbol);
            IEnumerable<AccessModifier> constructorAccessibilities = m_constructors.Select(c => c.Accessibility);

            Accessibility = AccessModifier.MostRestrictive([returnTypeAccessibility, .. constructorAccessibilities]);
        }

        /// <summary>
        /// Gets all the diagnostics that this class has produced
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            Options options = new Options();
            UnmarkedFactoryDiagnosticBuilder unmarkedFactoryDiagnostic = new UnmarkedFactoryDiagnosticBuilder(options);
            InconsistentFactoryAcessibilityBuilder inconsistentFactoryAcessibility = new InconsistentFactoryAcessibilityBuilder();
            // UnmarkedFactory
            if (!HasMarkerAttribute)
            {
                foreach (Diagnostic diagnostic in Constructors
                     .SelectMany(c => c.Parameters)
                     .Where(p => p.HasMarkerAttribute)
                     .Select(unmarkedFactoryDiagnostic.Build))
                {
                    yield return diagnostic;
                }
            }

            // ExposeAs is not a base type of Factory 
            if (!ExposeAsIsDereived())
            {
                ExposedAsNotDerivedTypeDiagnosticBuilder notDerivedBuilder = new ExposedAsNotDerivedTypeDiagnosticBuilder();
                yield return notDerivedBuilder.Build(ExposeAsLocation, m_typeSymbol, m_returnTypeSymbol);
            }

            // InconsistentFactoryAcessibility
            if (AccessModifier == AccessModifier.Internal &&
                FactoryAccessModifier == AccessModifier.Public)
            {
                yield return inconsistentFactoryAcessibility.Build(this);
            }
        }

        private bool ExposeAsIsDereived()
        {
            SymbolEqualityComparer comparer = SymbolEqualityComparer.Default;
            INamedTypeSymbol? typeSymbol = m_typeSymbol;
            while (typeSymbol != null)
            {
                if (comparer.Equals(m_returnTypeSymbol, typeSymbol))
                {

                    return true;
                }

                if (typeSymbol.Interfaces.Any(a => comparer.Equals(a, m_returnTypeSymbol)))
                {
                    return true;
                }

                typeSymbol = typeSymbol.BaseType;
            }
            return false;
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

                    if (SyntaxHelpers.TryPositionalArgument(attributeSyntax, 0, out AttributeArgumentSyntax? factoryTypeArg))
                    {
                        FactoryTypeLocation = factoryTypeArg.GetLocation();
                        if (SyntaxHelpers.GetValue(factoryTypeArg, m_semanticModel) is INamedTypeSymbol factoryTypeSymbol)
                        {
                            FactoryType = new MetadataTypeName(factoryTypeSymbol.ToDisplayString());
                            FactoryAccessModifier = AccessModifier.FromSymbol(factoryTypeSymbol);
                        }
                    }

                    if (SyntaxHelpers.TryPositionalArgument(attributeSyntax, 1, out AttributeArgumentSyntax? methodNameArg))
                    {
                        MethodNameLocation = methodNameArg.GetLocation();
                        if (SyntaxHelpers.GetValue(methodNameArg, m_semanticModel) is string methodName)
                        {
                            MethodName = methodName;
                        }
                    }

                    if (SyntaxHelpers.TryGetNamedArgument(attributeSyntax, "ExposeAs", out AttributeArgumentSyntax? exposeAsArg))
                    {
                        if (SyntaxHelpers.GetValue(exposeAsArg, m_semanticModel) is INamedTypeSymbol exposeAsTypeSymbol)
                        {
                            ExposeAsLocation = exposeAsArg.GetLocation();
                            m_returnTypeSymbol = exposeAsTypeSymbol;
                        }
                    }
                    return;
                }
            }
        }

        private void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (m_returnTypeSymbol is not null)
            {
                ConstructorDeclarationVisitor visitor = new(m_isAnalyzer, this, m_options, m_returnTypeSymbol, m_semanticModel);
                visitor.VisitConstructorDeclaration(node);
                m_constructors.Add(visitor);
            }
        }
    }
}
