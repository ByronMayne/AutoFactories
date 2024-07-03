using Microsoft.CodeAnalysis;

namespace Ninject.AutoFactories
{
    internal class GeneratorDriverResultFilter
    {
        public GeneratorDriverRunResult Result { get; }
        public Predicate<string>? Filter { get; }

        public GeneratorDriverResultFilter(GeneratorDriverRunResult result, Predicate<string>? filter)
        {
            Result = result;
            Filter = filter;
        }

        public bool Include(string filePath)
        {
            if (Filter is not null && !Filter(filePath))
            {
                return false;
            }

            return true;
        }
    }
}
