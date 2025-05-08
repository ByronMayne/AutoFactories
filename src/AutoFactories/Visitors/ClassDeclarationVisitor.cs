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
using System.Text;

namespace AutoFactories.Visitors
{
    [DebuggerDisplay("{Type,nq}")]
    internal class ClassDeclarationVisitor : SyntaxVisitor<ClassDeclarationSyntax>
    {
        private readonly List<ConstructorDeclarationVisitor> m_constructors;
        private readonly bool m_isAnalyzer;
        private readonly SemanticModel m_semanticModel;

        public bool HasMarkerAttribute { get; private set; }
        public string MethodName { get; private set; }
        public MetadataTypeName Type { get; private set; }
        public AccessModifier TypeAccessModifier { get; private set; }
        public AccessModifier InterfaceAccessModifier { get; private set; }
        public List<string> Usings { get; private set; }

        public MetadataTypeName FactoryType { get; private set; }

        public Location? MethodNameLocation { get; private set; }
        public Location? FactoryTypeLocation { get; private set; }
        public Location? ExposeAsLocation { get; private set; }

        public IReadOnlyList<ConstructorDeclarationVisitor> Constructors => m_constructors;

        /// <summary>
        /// Gets the access modifier that the constructor will need to have
        /// </summary>
        public AccessModifier FactoryAccessModifier { get; private set; }

        private INamedTypeSymbol? m_typeSymbol;
        private INamedTypeSymbol? m_returnTypeSymbol;

        public ClassDeclarationVisitor(
            bool isAnalyzer,
            SemanticModel semanticModel)
        {
            MethodName = "";
            Usings = new List<string>();
            m_constructors = [];
            m_isAnalyzer = isAnalyzer;
            m_semanticModel = semanticModel;
        }

        protected override void Visit(ClassDeclarationSyntax syntax)
        {
            if (m_semanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol typeSymbol)
            {
                return;
            }
            Type = new MetadataTypeName(typeSymbol.ToDisplayString());
            m_typeSymbol = typeSymbol;
            m_returnTypeSymbol = typeSymbol; // Default to current type 
            TypeAccessModifier = AccessModifier.FromSymbol(typeSymbol);
            InterfaceAccessModifier = TypeAccessModifier;
            FactoryAccessModifier = TypeAccessModifier;
            FactoryType = new MetadataTypeName($"{Type}Factory");


 
            Usings = syntax.SyntaxTree.GetRoot()
             .DescendantNodes()
             .OfType<UsingDirectiveSyntax>()
             .Select(u => u.ToString())
             .Select(s => s.Substring(6)) // remove 'using'
             .Select(s => s.Trim(' ', ';', '"'))
             .ToList();

            foreach (AttributeListSyntax attributeList in syntax.AttributeLists)
            {
                VisitAttributeList(attributeList);
            }

            if (!HasMarkerAttribute && !m_isAnalyzer)
            {
                // Analyzers will keep scanning for errors 
                return;
            }

            foreach (SyntaxNode childNode in syntax.ChildNodes())
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
                m_constructors.Add(new ConstructorDeclarationVisitor(
                    m_isAnalyzer,
                    this,
                    m_typeSymbol,
                    m_returnTypeSymbol,
                    m_semanticModel));
            }

            AccessModifier returnTypeAccessibility = AccessModifier.FromSymbol(m_returnTypeSymbol);
            IEnumerable<AccessModifier> constructorAccessibilities = m_constructors.Select(c => c.Accessibility);

            FactoryAccessModifier = AccessModifier.MostRestrictive([returnTypeAccessibility, .. constructorAccessibilities]);

            PopulateDiagnostics();
        }

        private void PopulateDiagnostics()
        {
            // UnmarkedFactory
            if (!HasMarkerAttribute)
            {
                foreach (ParameterSyntaxVisitor parameter in Constructors
                     .SelectMany(c => c.Parameters)
                     .Where(p => p.HasMarkerAttribute))
                {
                    AddDiagnostic(UnmarkedFactoryDiagnostic.Create(parameter));
                }
            }

            // ExposeAs is not a base type of Factory 
            if (!ExposeAsIsDerived())
            {
                AddDiagnostic(ExposedAsNotDerivedTypeDiagnostic.Create(ExposeAsLocation, m_typeSymbol, m_returnTypeSymbol));
            }

            // InconsistentFactoryAccessibility
            if (TypeAccessModifier == AccessModifier.Internal &&
                FactoryAccessModifier == AccessModifier.Public)
            {
                AddDiagnostic(InconsistentFactoryAccessibilityBuilder.Create(this));
            }

        }

        private bool ExposeAsIsDerived()
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

                if (string.Equals(TypeNames.ClassAttributeType.QualifiedName, displayString))
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
                            TypeAccessModifier = AccessModifier.FromSymbol(exposeAsTypeSymbol);
                            m_returnTypeSymbol = exposeAsTypeSymbol;
                        }
                    }
                    return;
                }
            }
        }

        private void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (m_returnTypeSymbol is not null && m_typeSymbol is not null)
            {
                ConstructorDeclarationVisitor visitor = new(
                    m_isAnalyzer,
                    this,
                    m_typeSymbol,
                    m_returnTypeSymbol,
                    m_semanticModel);
                visitor.Accept(node);
                m_constructors.Add(visitor);
                AddChild(visitor);
            }
        }
    }
}
