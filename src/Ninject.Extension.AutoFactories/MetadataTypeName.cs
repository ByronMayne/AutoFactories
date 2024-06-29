using System.Text.RegularExpressions;

namespace Ninject.Extension.AutoFactories
{
    internal struct MetadataTypeName
    {
        private static readonly string s_attributePostfix;
        private static readonly Regex s_splitRegex;
        private readonly string m_shortName;

        public readonly string TypeName;
        public readonly string? Namespace;
        public readonly string FullName;

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
            TypeName = match.Groups["ClassName"].Value;
            Namespace = match.Groups["Namespace"].Value;
            FullName = string.IsNullOrWhiteSpace(Namespace)
                ? TypeName
                : $"{Namespace}.{TypeName}";

            if (TypeName.EndsWith("Attribute"))
            {
                m_shortName = TypeName.Substring(0, TypeName.Length - s_attributePostfix.Length);
            }
        }

        public bool IsEqualavanetTypeName(string? name)
        {
            if (name == null)
            {
                return false;
            }

            if (string.Equals(name, TypeName))
            {
                return true;
            }

            return string.Equals(name, m_shortName);
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
