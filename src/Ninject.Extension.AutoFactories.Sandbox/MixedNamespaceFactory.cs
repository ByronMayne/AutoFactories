using AutoFactories;
using System.Collections;
using System.Collections.Specialized;

namespace Foo
{
    /// <summary>
    /// Test case for https://github.com/ByronMayne/Ninject.Extensions.AutoFactories/issues/14
    /// Namespaces for parameters were not being written using their fully qualifed name leading to 
    /// this class being a compiler error.
    /// </summary>
    [AutoFactory]
    public class ExampleClass
    {
        public ExampleClass(
            string name,
            [FromFactory] IList<string> people,
            OrderedDictionary dictionary,
            [FromFactory] IEqualityComparer equalityComparer) 
        {

        }
    }
}