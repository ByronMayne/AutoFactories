using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace AutoFactories.Tests.Generators
{
    internal class AddSourceIncrementalGenerator : IIncrementalGenerator
    {
        public string HintName { get; }
        public SourceText SourceText { get; }

        public AddSourceIncrementalGenerator(string source, string hintName)
            : this(SourceText.From(source, encoding: Encoding.UTF8), hintName)
        { }

        public AddSourceIncrementalGenerator(SourceText sourceText, string hintName)
        {
            HintName = hintName;
            SourceText = sourceText;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(
                context.CompilationProvider,
                AddSource);
        }

        private void AddSource(SourceProductionContext context, Compilation compilation)
        {
            context.AddSource(HintName, SourceText);    
        }
    }
}
