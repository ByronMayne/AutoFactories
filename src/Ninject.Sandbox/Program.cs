using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ninject.Sandbox
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            StandardKernel kernel = new StandardKernel()
                .LoadFactories();

            IEnumerable<INinjectModule> modules = kernel.GetModules();
            NinjectSettings settings = new NinjectSettings();
        }
    }
}
