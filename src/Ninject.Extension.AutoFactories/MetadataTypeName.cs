using System.Text.RegularExpressions;

namespace Ninject.Extension.AutoFactories
{
    internal struct MetadataTypeName
    {
        private static readonly Regex s_splitRegex;

        public readonly string TypeName;
        public readonly string? Namespace;
        public readonly string FullName;

        static MetadataTypeName()
        {
            s_splitRegex = new Regex("^((?<Namespace>.*)(?:\\.))?(?<ClassName>.*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        }

        public MetadataTypeName(string value)
        {
            Match match = s_splitRegex.Match(value);
            if (!match.Success)
            {
                throw new FormatException("The string was not in the expected format");
            }
            TypeName = match.Groups["ClassName"].Value;
            Namespace = match.Groups["Namespace"].Value;
            FullName = string.IsNullOrWhiteSpace(Namespace)
                ? TypeName
                : $"{Namespace}.{TypeName}";
        }
    }
}
