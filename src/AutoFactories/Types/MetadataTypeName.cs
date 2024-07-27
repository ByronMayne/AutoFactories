using System;
using System.Text.RegularExpressions;

namespace AutoFactories.Types
{
    internal struct MetadataTypeName
    {
        private static readonly string s_attributePostfix;
        private static readonly Regex s_splitRegex;
        private readonly string m_shortName;

        public readonly string Name;
        public readonly string? Namespace;
        public readonly string QualifedName;

        static MetadataTypeName()
        {
            s_attributePostfix = "Attribute";
            s_splitRegex = new Regex("^((?<Namespace>.*)(?:\\.))?(?<ClassName>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        }

        public MetadataTypeName(string value)
        {
            Match match = s_splitRegex.Match(value);
            if (!match.Success)
            {
                throw new FormatException("The string was not in the expected format");
            }
            m_shortName = "";
            Name = match.Groups["ClassName"].Value;
            Namespace = match.Groups["Namespace"].Value;
            QualifedName = string.IsNullOrWhiteSpace(Namespace)
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
            return QualifedName;
        }
    }
}
