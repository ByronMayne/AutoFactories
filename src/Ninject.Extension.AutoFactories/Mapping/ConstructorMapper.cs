using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;

namespace Ninject.AutoFactories.Mapping
{
    internal class ConstructorMapper : IMapper<ConstructorDeclarationSyntax, ConstructorModel>
    {
        private readonly IMapper<ParameterSyntax, ParameterModel> m_parameterMapper;

        public ConstructorMapper(SemanticModel semanticModel)
        {
            m_parameterMapper = new ParameterMapper(semanticModel);
        }

        public void Map(ConstructorDeclarationSyntax source, ConstructorModel destination)
        {
            ParameterSyntax[] parameters = source.ParameterList.Parameters
                .Where(p => !SyntaxHelpers.HasAttribute(p, "FromFactoryAttribute"))
                .ToArray();

            destination.Parameters = m_parameterMapper.MapList(parameters);
        }
    }
}
