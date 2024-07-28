using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AutoFactories.Extensions
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
                foreach (AttributeSyntax attribute in attributeList.Attributes)
                {
                    string text = attribute.Name.ToString();
                    if (string.Equals(fullName, text, stringComparison) ||
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
            while (potentialNamespaceParent is not null and
                    not NamespaceDeclarationSyntax
                    and not FileScopedNamespaceDeclarationSyntax)
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

        public static object? GetValue(this AttributeArgumentSyntax instance, SemanticModel semanticModel)
        {
            switch (instance.Expression)
            {
                case InterpolatedStringExpressionSyntax interpolatedString:
                    {

                        Optional<object?> optional = semanticModel.GetConstantValue(interpolatedString);
                        return optional.Value;
                    }
                case TypeOfExpressionSyntax typeOfExpression:
                    {
                        SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(typeOfExpression.Type);
                        ISymbol? symbol = symbolInfo.Symbol;
                        return symbol;
                    }
                case LiteralExpressionSyntax literalExpression:
                    {
                        Optional<object?> optional = semanticModel.GetConstantValue(literalExpression);
                        return optional.Value;
                    }
            }

            return null;
        }


        public static bool TryPositionalArgument(this AttributeSyntax attributeSyntax, int position, [NotNullWhen(true)] out AttributeArgumentSyntax? result)
        {
            result = null;

            if (attributeSyntax.ArgumentList is null)
            {
                return false;
            }

            for (int i = 0; i < attributeSyntax.ArgumentList.Arguments.Count; i++)
            {
                AttributeArgumentSyntax argument = attributeSyntax.ArgumentList.Arguments[i];

                if (argument.NameEquals is not null)
                {
                    return false;
                }

                if (argument.NameColon is not null)
                {
                    return false;
                }

                if (i == position)
                {
                    result = argument;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to find an argument where users us the `name: value` method
        /// </summary>
        public static bool TryGetNameColonArgument(this AttributeSyntax? attributeSyntax, string argumentName, [NotNullWhen(true)] out AttributeArgumentSyntax? result)
        {
            result = null;

            if (attributeSyntax == null)
            {
                return false;
            }

            if (attributeSyntax.ArgumentList == null)
            {
                return false;
            }

            foreach (AttributeArgumentSyntax value in attributeSyntax.ArgumentList.Arguments)
            {
                if (value.NameColon is not NameColonSyntax nameColone)
                {
                    continue;
                }

                if (string.Equals(nameColone.Name.Identifier.Text, argumentName))
                {
                    result = value;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetNamedArgument(this AttributeSyntax? attributeSyntax, string argumentName, [NotNullWhen(true)] out AttributeArgumentSyntax? result)
        {
            result = null;

            if (attributeSyntax == null)
            {
                return false;
            }

            if (attributeSyntax.ArgumentList == null)
            {
                return false;
            }

            foreach (AttributeArgumentSyntax value in attributeSyntax.ArgumentList.Arguments)
            {
                if (value.NameEquals is not NameEqualsSyntax nameEquals)
                {
                    continue;
                }

                if (string.Equals(nameEquals.Name.Identifier.Text, argumentName))
                {
                    result = value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the string value of of an argument
        /// </summary>
        public static bool TryGetStringValue(this AttributeArgumentSyntax? attributeSyntax, SemanticModel semanticModel, [NotNullWhen(true)] out string? result)
        {
            result = null;
            if (attributeSyntax is null)
            {
                return false;
            }

            Optional<object?> constantValue = semanticModel.GetConstantValue(attributeSyntax.Expression);

            if (!constantValue.HasValue)
            {
                return false;
            }

            result = constantValue.ToString();
            return !string.IsNullOrWhiteSpace(result);
        }


        //public static T? GetNamedArgumentValue<T>(this AttributeSyntax? attribute, string argumentName, SemanticModel semanticModel, T? defaultValue)
        //{
        //    if (attribute == null)
        //    {
        //        return defaultValue;
        //    }

        //    if (attribute.ArgumentList == null)
        //    {
        //        return defaultValue;
        //    }

        //    foreach (AttributeArgumentSyntax value in attribute.ArgumentList.Arguments)
        //    {
        //        if (value.NameEquals is not NameEqualsSyntax nameEquals)
        //        {
        //            continue;
        //        }

        //        if (string.Equals(nameEquals.Name.Identifier.Text, argumentName))
        //        {
        //            Optional<object?> constantValue = semanticModel.GetConstantValue(value.Expression);

        //            if (constantValue.HasValue)
        //            {
        //                return (T)constantValue.Value!;
        //            }
        //        }
        //    }
        //    return defaultValue;
        //}
    }
}
