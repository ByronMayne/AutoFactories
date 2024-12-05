using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Ninject.AutoFactories
{
    public readonly struct AccessModifier : IComparable<AccessModifier>, IEquatable<AccessModifier>
    {
        public static readonly AccessModifier Public = new AccessModifier("public", 6);
        public static readonly AccessModifier Internal = new AccessModifier("internal", 4);
        public static readonly AccessModifier Protected = new AccessModifier("protected", 3);
        public static readonly AccessModifier ProtectedAndInternal = new AccessModifier("protected internal", 2);
        public static readonly AccessModifier Private = new AccessModifier("private", 1);

        public readonly string Value;
        public readonly int Order;


        private AccessModifier(string value, int order)
        {
            Value = value;
            Order = order;
        }

        public static AccessModifier MostRestrictive(params AccessModifier[] modifiers)
        {
            AccessModifier result = modifiers[0];
            for(int i = 1; i < modifiers.Length; i++)
            {
                if (modifiers[i].CompareTo(result) > 0)
                {
                    result = modifiers[i];
                }
            }
            return result;
        }

        public static AccessModifier FromSymbol(ISymbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                default:
                case Accessibility.Internal: return Internal;
                case Accessibility.Public: return Public;
                case Accessibility.Protected: return Protected;
                case Accessibility.ProtectedAndInternal: return ProtectedAndInternal;
                case Accessibility.Private: return Private;
            }
        }

        public bool Equals(AccessModifier other, IEqualityComparer<AccessModifier> comparer)
        {
            return comparer.Equals(this, other);
        }

        public readonly bool Equals(string primitive)
        {
            return Value.Equals(primitive);
        }

        public readonly bool Equals(string primitive, StringComparer comparer)
        {
            return comparer.Equals(Value, primitive);
        }


        public bool Equals(AccessModifier other)
            => other.Order == Order;

        public readonly override bool Equals(object obj)
            => obj is AccessModifier other && Equals(other);

        public int CompareTo(AccessModifier other)
            => other.Order.CompareTo(Order);

        public override string ToString()
            => Value;

        public override int GetHashCode()
        {
            int hashCode = -1519344079;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + Order.GetHashCode();
            return hashCode;
        }


        public static bool operator ==(AccessModifier left, AccessModifier right) => left.Equals(right);
        public static bool operator !=(AccessModifier left, AccessModifier right) => !(left == right);
        public static bool operator ==(AccessModifier left, string right) => Equals(left.Value, right);
        public static bool operator !=(AccessModifier left, string right) => !Equals(left.Value, right);
        public static bool operator ==(string left, AccessModifier right) => Equals(left, right.Value);
        public static bool operator !=(string left, AccessModifier right) => !Equals(left, right.Value);
    }
}
