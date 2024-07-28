using AutoFactories.Types;
using AutoFactories.Visitors;
using System.Collections.Generic;

namespace AutoFactories.Views.Models
{
    internal class ParameterModel
    {
        public class EqualityComparer : IEqualityComparer<ParameterModel>
        {
            bool IEqualityComparer<ParameterModel>.Equals(ParameterModel x, ParameterModel y)
            {
                return string.Equals(x.Name, y.Name, System.StringComparison.Ordinal) &&
                    x.Type.QualifiedName.Equals(y.Type.QualifiedName);
            }

            int IEqualityComparer<ParameterModel>.GetHashCode(ParameterModel parameter)
            {
                return parameter.GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the name of the parameter 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter 
        /// </summary>
        public MetadataTypeName Type { get; set; }
        
        /// <summary>
        /// Gets or sets if the parameter has to be provided by the invokee 
        /// </summary>
        public bool IsRequired { get; set; }

        public ParameterModel()
        {
            Name = "";
        }


        public static ParameterModel Map(ParameterSyntaxVisitor visitor)
            => new ParameterModel()
            {
                Name = visitor.Name!,
                Type = visitor.Type,
                IsRequired = !visitor.HasMarkerAttribute
            };
    }
}
