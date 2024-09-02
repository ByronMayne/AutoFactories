using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoFactories.Types
{
    internal struct MetadataTypeName
    {
        private static IDictionary<string, MetadataTypeName> s_typeAliases;
        private static readonly string s_attributePostfix;
        private static readonly Regex s_splitRegex;
        private readonly string m_shortName;

        public readonly string Name;
        public readonly string? Namespace;
        public readonly string QualifiedName;


        static MetadataTypeName()
        {
            s_attributePostfix = "Attribute";
            s_splitRegex = new Regex("^((?<Namespace>.*)(?:\\.))?(?<ClassName>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            s_typeAliases = new Dictionary<string, MetadataTypeName>();

            AddAlias<string>("string");
            AddAlias<int>("int");
            AddAlias<long>("long");
            AddAlias<float>("float");
            AddAlias<double>("double");
            AddAlias<short>("short");
            AddAlias<bool>("bool");
            AddAlias<uint>("uint");
            AddAlias<ulong>("ulong");
            AddAlias<ushort>("ushort");
        }

        static void AddAlias<T>(string alias)
        {
            s_typeAliases[alias] = new MetadataTypeName(typeof(T).FullName);
        }

        public MetadataTypeName(string value)
        {
            if(s_typeAliases.TryGetValue(value, out MetadataTypeName alias))
            {
                this = alias;
                return;
            }

            Match match = s_splitRegex.Match(value);
            if (!match.Success)
            {
                throw new FormatException("The string was not in the expected format");
            }
            m_shortName = "";
            Name = match.Groups["ClassName"].Value;
            Namespace = match.Groups["Namespace"].Value;
            QualifiedName = string.IsNullOrWhiteSpace(Namespace)
                ? Name
                : $"{Namespace}.{Name}";

            if (Name.EndsWith("Attribute"))
            {
                m_shortName = Name.Substring(0, Name.Length - s_attributePostfix.Length);
            }
        }

        public bool IsEqualavanetTypeName(string? name)
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
