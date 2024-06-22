using System.Diagnostics;

namespace Ninject.AutoFactory.Models
{
    [DebuggerDisplay("{Name,nq} {Type,nq}")]
    internal class ParameterModel
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsFactory { get; set; } = false;
    }
}
