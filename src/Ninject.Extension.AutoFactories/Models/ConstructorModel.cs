using Ninject.AutoFactory.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.Extension.AutoFactories.Models
{
    internal class ConstructorModel
    {
        public string Name { get; set; }
        public IReadOnlyList<ParameterModel> Parameters { get; set; }


        public ConstructorModel()
        {
            Name = "Create";
            Parameters = Array.Empty<ParameterModel>(); 
        }
    }
}
