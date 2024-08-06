using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;

namespace AutoFactories
{
    internal class HandlebarsTemplateText
    {
        private readonly AdditionalText m_backingField;

        public string Path { get; }
        public string FileName { get; }

        public HandlebarsTemplateText(AdditionalText backingField)
        {
            m_backingField = backingField;
            Path = m_backingField.Path;
            FileName = System.IO.Path.GetFileNameWithoutExtension(m_backingField.Path);
        }

        public string GetText()
        {
            SourceText? sourceText = m_backingField.GetText();
            return sourceText!.ToString();
        }

        public static HandlebarsTemplateText Create(AdditionalText additionalText, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new HandlebarsTemplateText(additionalText);
        }
    }
}
