using AutoFactories.Types;
using AutoFactories.Visitors;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Models
{
    internal class FactoryMethodViewModel
    {
        public string Name { get; set; }
        public MetadataTypeName Type { get; set; }
        public MetadataTypeName ReturnType { get; set; }
        public List<ParameterViewModel> Parameters { get; set; }
        public AccessModifier Accessibility { get; set; }
        public IReadOnlyList<ParameterViewModel> RequiredParameters
            => Parameters.Where(p => p.IsRequired).ToList();

        public FactoryMethodViewModel()
        {
            Name = "";
            Parameters = new List<ParameterViewModel>();
        }

        public static FactoryMethodViewModel Map(ConstructorDeclarationVisitor vistor)
               => new FactoryMethodViewModel()
               {
                   Name = "Create",
                   Type = vistor.Type,
                   Parameters = vistor.Parameters.Select(ParameterViewModel.Map).ToList(),
                   ReturnType = vistor.ReturnType,
               };
    }
}
