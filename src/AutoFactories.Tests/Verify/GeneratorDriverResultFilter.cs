using Microsoft.CodeAnalysis;

namespace Ninject.AutoFactories
{
    /// <summary>
    /// Custom converter for files which allows us to restrict the test to a spesefic view
    /// </summary>
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

        public static void Initialize()
        {
            VerifierSettings.RegisterFileConverter<GeneratorDriverResultFilter>(Convert);
        }

        private static ConversionResult Convert(GeneratorDriverResultFilter target, IReadOnlyDictionary<string, object> context)
        {
            var exceptions = new List<Exception>();
            var targets = new List<Target>();
            foreach (var result in target.Result.Results)
            {
                if (result.Exception != null)
                {
                    exceptions.Add(result.Exception);
                }

                var collection = result.GeneratedSources
                    .Where(x => target.Include(x.HintName))
                    .OrderBy(x => x.HintName)
                    .Select(SourceToTarget);

                if (!collection.Any())
                {
                    Assert.Fail($"No tests subjects matched any of the patterns. The following subjects were found\n{string.Join("\n - ", result.GeneratedSources.Select(s => s.HintName))}");
                }

                targets.AddRange(collection);
            }

            if (exceptions.Count == 1)
            {
                throw exceptions.First();
            }

            if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }

            if (target.Result.Diagnostics.Any())
            {
                var info = new
                {
                    target.Result.Diagnostics
                };
                return new(info, targets);
            }

            return new(null, targets);
        }

        private static Target SourceToTarget(GeneratedSourceResult source)
        {
            var data = $"""
            //HintName: {source.HintName}
            {source.SourceText}
            """;
            return new("cs", data, Path.GetFileNameWithoutExtension(source.HintName));
        }
    }
}
