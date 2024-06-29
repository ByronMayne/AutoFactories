using Boxed.Mapping;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactory.Models;
using Ninject.Extension.AutoFactories.Models;

namespace Ninject.AutoFactory.Mapping
{
    internal class ConstructorMapper : IMapper<ConstructorDeclarationSyntax, ConstructorModel>
    {
        private readonly IMapper<ParameterSyntax, ParameterModel> m_parameterMapper;

        public ConstructorMapper()
        {
            m_parameterMapper = new ParameterMapper();
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
