using AutoFactories.Types;
using AutoFactories.Views.Models;
using Ninject.AutoFactories;
using System.Collections.Generic;

namespace AutoFactories.Views
{
    internal class FactoryView : View
    {
        public MetadataTypeName Type { get; set; }
        public AccessModifier AccessModifier { get; set; }

        public IList<MethodModel> Methods { get; set; }

        public IList<ParameterModel> Parameters { get; set; }

        public FactoryView() 
        {
            Methods = new List<MethodModel>();
            Parameters = new List<ParameterModel>();
        }
    }
}
