using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;

namespace Ninject.AutoFactories.Mapping
{
    internal class ParameterMapper : IMapper<ParameterSyntax, ParameterModel>
    {
        private readonly SemanticModel m_semanticModel;

        public ParameterMapper(SemanticModel semanticModel)
        {
            m_semanticModel = semanticModel;
        }

        public void Map(ParameterSyntax source, ParameterModel destination)
        {
            SymbolInfo symbolInfo = m_semanticModel
                .GetSymbolInfo(source.Type!);
          

            destination.Name = source.Identifier.Text;
            destination.Type = symbolInfo.Symbol.ToDisplayString();
        }
    }
}
