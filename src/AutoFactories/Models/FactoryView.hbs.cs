using AutoFactories.Types;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Models
{
    internal class FactoryView : View
    {
        public MetadataTypeName Type { get; set; }
        public AccessModifier AccessModifier { get; set; }

        public IList<MethodModel> Methods { get; set; }

        public IList<ParameterModel> Parameters { get; set; }

        public IReadOnlyList<ParameterModel> RequiredParameters => Parameters.Where(p => p.IsRequired).ToList();

        public FactoryView()
        {
            Methods = new List<MethodModel>();
            Parameters = new List<ParameterModel>();
        }
    }
}
