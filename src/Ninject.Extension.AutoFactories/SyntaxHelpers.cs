using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Immutable;

namespace Ninject.AutoFactory
{
    internal static class SyntaxHelpers
    {
        private static readonly char[] s_nestedTypeNameSeparators = new char[] { '+' };

        /// <summary>
        /// Validates if a attribute with the given fully qualifed name is applied to the given parameter syntax.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="fullName">The name of the attribute</param>
        /// <returns></returns>
        public static bool HasAttribute(BaseParameterSyntax syntax, string fullName)
        {
            const string Attribute = nameof(Attribute);

            const StringComparison stringComparison = StringComparison.Ordinal;
            string alternativeName = fullName.EndsWith(Attribute)
                ? fullName.Substring(0, fullName.Length - Attribute.Length)
                : $"{fullName}Attribute";

            foreach (AttributeListSyntax attributeList in syntax.AttributeLists)
            {
                foreach(AttributeSyntax attribute in attributeList.Attributes)
                {
                    string text = attribute.Name.ToString();
                    if(string.Equals(fullName, text, stringComparison) ||
                       string.Equals(alternativeName, text, stringComparison))
                    {
                        return true;
                    }
                
                }
            }
            return false;
        }

        public static string GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode? potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }
    
        public static T GetArgumentValue<T>(this AttributeSyntax? attribute, string argumentName, SemanticModel semanticModel, T defaultValue)
        {
            if (attribute == null) return defaultValue;
            if (attribute.ArgumentList == null) return defaultValue;

            foreach (AttributeArgumentSyntax value in attribute.ArgumentList.Arguments)
            {
                if (value.NameEquals is not NameEqualsSyntax nameEquals) continue;
            
                if(string.Equals(nameEquals.Name.Identifier.Text, argumentName))
                {
                    Optional<object?> constantValue = semanticModel.GetConstantValue(value.Expression);

                    if (constantValue.HasValue)
                    {
                        return (T)constantValue.Value!;
                    }
                }
            }
            return defaultValue;
        }
    }
}
