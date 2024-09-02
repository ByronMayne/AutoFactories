using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutoFactories.CodeAnalysis
{
    internal class HandlebarsText : AdditionalText
    {
        private readonly SourceText? m_sourceText;
        public override string Path { get; }

        public HandlebarsText(string path, string text)
        {
            Path = path;
            m_sourceText = SourceText.From(text, Encoding.UTF8);
        }

        public HandlebarsText(AdditionalText original)
        {
            Path = original.Path;
            m_sourceText = original.GetText();
        }

        public override SourceText? GetText(CancellationToken cancellationToken = default)
            => m_sourceText;
    }
}
