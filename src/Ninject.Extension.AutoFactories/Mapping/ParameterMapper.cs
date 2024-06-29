using Boxed.Mapping;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactories.Models;

namespace Ninject.AutoFactories.Mapping
{
    internal class ParameterMapper : IMapper<ParameterSyntax, ParameterModel>
    {
        public void Map(ParameterSyntax source, ParameterModel destination)
        {
            destination.Name = source.Identifier.Text;
            destination.Type = source.Type!.ToString();
        }
    }
}
