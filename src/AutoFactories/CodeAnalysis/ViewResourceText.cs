using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AutoFactories.CodeAnalysis
{
    [DebuggerDisplay("{Kind} | {Path}")]
    internal class ViewResourceText : AdditionalText
    {
        private static Regex s_regex;

        static ViewResourceText()
        {
            RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
            s_regex = new Regex(@"views[\\\/](?<Type>[^\\\/]*)[\\\/].*\.hbs$", regexOptions);
        }

        private readonly SourceText? m_sourceText;
        public override string Path { get; }
        public ViewKind Kind { get; }
        public ViewKey Key { get; }


        public ViewResourceText(
            string path, 
            string text)
        {
            Path = path;
            Key = ViewKey.From(System.IO.Path.GetFileNameWithoutExtension(path));
            if (!TryGetViewType(path, out ViewKind templateKind))
            {
                throw new ArgumentException("Not a valid template kind");
            }
            Kind = templateKind;
            m_sourceText = SourceText.From(text, Encoding.UTF8);
        }

        private ViewResourceText(AdditionalText original) : this(original.Path, original.GetText()!.ToString())
        { }

        public override SourceText? GetText(CancellationToken cancellationToken = default)
            => m_sourceText;


        /// <summary>
        /// Attempts to parse out a template file 
        /// </summary>
        /// <param name="original">The base text to try to parse</param>
        /// <param name="result">The template text if it was parsed</param>
        /// <returns>True if it was false if it was not.</returns>
        public static bool TryParse(
            AdditionalText original,
            [NotNullWhen(true)] out ViewResourceText? result)
        {
            result = null;

            if(TryGetViewType(original.Path, out ViewKind templateKind) &&
                templateKind != ViewKind.None)
            {
                result = new ViewResourceText(original);
                return true;
            }
            return false;
        }

        private static bool TryGetViewType(string filePath, out ViewKind templateKind)
        {
            templateKind = ViewKind.None;

            Match match = s_regex.Match(filePath);
            if (match.Success)
            {
                templateKind = (ViewKind)Enum.Parse(typeof(ViewKind), match.Groups["Type"].Value);
                return true;
            }
            return false;
        }
    }
}
