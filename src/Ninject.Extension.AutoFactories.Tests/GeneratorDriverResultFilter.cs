using Microsoft.CodeAnalysis;

namespace Ninject.AutoFactories
{
    internal class GeneratorDriverResultFilter
    {
        public GeneratorDriverRunResult Result { get; }
        public IReadOnlyList<Predicate<string>> Filters { get; }

        public GeneratorDriverResultFilter(GeneratorDriverRunResult result, Predicate<string>[]? filters)
        {
            Result = result;
            Filters = filters ?? Array.Empty<Predicate<string>>();  
        }

        public bool Include(string filePath)
        {
            foreach (Predicate<string> filter in Filters)
            {
                if (!filter(filePath))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
