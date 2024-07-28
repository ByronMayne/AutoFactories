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
        public List<ParameterModel> RequiredParameters { get; set; }
        public List<ParameterModel> ProvidedParameters { get; set; }

        public MethodModel()
        {
            Name = "";
            RequiredParameters = new List<ParameterModel>();
            ProvidedParameters = new List<ParameterModel>();
        }

        public static MethodModel Map(ConstructorDeclarationVisitor vistor)
               => new MethodModel()
               {
                   Name = "Create",
                   ProvidedParameters = vistor.Parameters.Where(p => p.HasMarkerAttribute).Select(ParameterModel.Map).ToList(),
                   RequiredParameters = vistor.Parameters.Where(p => !p.HasMarkerAttribute).Select(ParameterModel.Map).ToList(),
                   ReturnType = vistor.ReturnType,
               };
    }
}
