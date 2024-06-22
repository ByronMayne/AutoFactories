using System;
using System.Collections.Generic;
using System.Text;

namespace Ninject.AutoFactory.Models
{
    internal class MethodModel
    {
        public List<ParameterModel> Parameters { get; set; }

        public MethodModel()
        {
            Parameters = new List<ParameterModel>(); 
        }
    }
}
