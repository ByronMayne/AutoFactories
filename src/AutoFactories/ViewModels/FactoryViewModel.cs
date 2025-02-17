using AutoFactories.Types;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Models
{
    internal class FactoryViewModel : ViewModel
    {
        public MetadataTypeName Type { get; set; }
        public AccessModifier InterfaceAccessModifier { get; set; }
        public AccessModifier ImplementationAccessModifier { get; set; }

        public IList<string> Usings { get; set; }

        public IList<FactoryMethodViewModel> Methods { get; set; }

        public IList<ParameterViewModel> Parameters { get; set; }

        public IReadOnlyList<ParameterViewModel> RequiredParameters => Parameters.Where(p => p.IsRequired).ToList();

        public FactoryViewModel()
        {
            Usings = new List<string>();
            Methods = new List<FactoryMethodViewModel>();
            Parameters = new List<ParameterViewModel>();
        }
    }
}
