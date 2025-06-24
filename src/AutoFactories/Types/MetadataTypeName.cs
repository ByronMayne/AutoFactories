using AutoFactories.Extensions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoFactories.Types
{
    internal struct MetadataTypeName
    {
        private readonly string m_shortName;

        public string Name { get; init; }
        public string? Namespace { get; init; }
        public string QualifiedName { get; init; }
        public bool IsNullable { get; init; }
        public bool IsAlias { get; init; }

        public MetadataTypeName(
            string name,
            string? @namespace,
            bool isNullable,
            bool isAlias)
        {
            Name = name;
            Namespace = @namespace;
            IsNullable = isNullable;
            IsAlias = isAlias;
            QualifiedName = Name;
            QualifiedName = string.IsNullOrEmpty(Namespace)
                ? Name
                : $"{Namespace}.{Name}";
        }


        public MetadataTypeName(ITypeSymbol typeSymbol)
        {
            this = SymbolHelpers.ResolveTypeName(typeSymbol);
        }

        public bool IsEquivalatedTypeName(string? name)
        {
            if (name == null)
            {
                return false;
            }

            return string.Equals(name, Name) || string.Equals(name, m_shortName);
        }

        public override string ToString()
        {
            return QualifiedName;
        }
    }
}
