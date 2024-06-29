using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SGF;
using System.Text;

namespace Ninject.AutoFactories.Templates
{
    internal abstract class Template
    {
        public string HintName { get; }

        public Encoding Encoding { get; }



        public Template(string hintNmae)
        {
            HintName = hintNmae;
            Encoding = Encoding.UTF8;
        }


        /// <summary>
        /// Adds this template toe the post initialize context callback
        /// </summary>
        /// <param name="context"></param>
        public void AddSource(IncrementalGeneratorPostInitializationContext context)
        {
            string content = Render();
            SourceText sourceText = SourceText.From(content, Encoding);
            context.AddSource(HintName, sourceText);
        }

        public void AddSource(SgfSourceProductionContext context)
        {
            string content = Render();
            SourceText sourceText = SourceText.From(content, Encoding);
            context.AddSource(HintName, sourceText);
        }

        protected abstract string Render();
    }
}
