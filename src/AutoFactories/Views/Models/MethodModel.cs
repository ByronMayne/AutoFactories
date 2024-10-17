using AutoFactories.Types;
using AutoFactories.Visitors;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Views.Models
{
    internal class MethodModel
    {
        public string Name { get; set; }
        public MetadataTypeName ReturnType { get; set; }
        public List<ParameterModel> Parameters { get; set; }
        public IReadOnlyList<ParameterModel> RequiredParameters
            => Parameters.Where(p => p.IsRequired).ToList();

        public MethodModel()
        {
            Name = "";
            Parameters = new List<ParameterModel>();
        }

        public static MethodModel Map(ConstructorDeclarationVisitor vistor)
               => new MethodModel()
               {
                   Name = "Create",
                   Parameters = vistor.Parameters.Select(ParameterModel.Map).ToList(),
                   ReturnType = vistor.ReturnType,
               };
    }
}
