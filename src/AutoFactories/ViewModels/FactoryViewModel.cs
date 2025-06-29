using AutoFactories.Types;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Models
{
    internal class FactoryViewModel : ViewModel
    {
        private MetadataTypeName m_type;

        public MetadataTypeName Type
        {
            get => m_type;
            set
            {
                m_type = value;
                InterfaceType = new MetadataTypeName($"I{m_type.Name}", m_type.Namespace, m_type.IsNullable, m_type.IsAlias);
            }
        }
        public MetadataTypeName InterfaceType { get; set; }
        public AccessModifier InterfaceAccessModifier { get; set; }
        public AccessModifier ImplementationAccessModifier { get; set; }

        public bool IsValid { get; set; }

        public IList<string> Usings { get; set; }

        public IList<FactoryMethodViewModel> Methods { get; set; }

        public IList<ParameterViewModel> Parameters { get; set; }

        public IReadOnlyList<ParameterViewModel> RequiredParameters => Parameters.Where(p => p.IsRequired).ToList();

        public FactoryViewModel()
        {
            IsValid = true;
            Usings = new List<string>();
            Methods = new List<FactoryMethodViewModel>();
            Parameters = new List<ParameterViewModel>();
        }
    }
}
