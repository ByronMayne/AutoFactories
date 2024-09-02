using Microsoft.CodeAnalysis;
using System.Text;

namespace Ninject.AutoFactories
{
    /// <summary>
    /// Custom converter for files which allows us to restrict the test to a spesefic view
    /// </summary>
    internal class GeneratorDriverResultFilter
    {
        public GeneratorDriverRunResult Result { get; }
        public Predicate<string>? Filter { get; }

        /// <summary>
        /// Gets or sets an optional note to add to the top of the file to explain what is unique about it.
        /// </summary>
        public List<string> Notes { get; set; }

        public GeneratorDriverResultFilter(
            GeneratorDriverRunResult result, 
            Predicate<string>? filter)
        {
            Result = result;
            Filter = filter;
            Notes = new List<string>();
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
                    .Select(x => SourceToTarget(x, target.Notes));

                if (!collection.Any())
                {
                    Assert.Fail($"Failed: Input was verfied but No test subjects were matched. The following subjects were found\n - {string.Join("\n - ", result.GeneratedSources.Select(s => s.HintName))}");
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

        private static Target SourceToTarget(GeneratedSourceResult source, IList<string> notes)
        {
            StringBuilder dataBuilder = new StringBuilder();
            dataBuilder.AppendFormat("//HintName: {0}", source.HintName).AppendLine();
            if(notes.Count > 0)
            {
                dataBuilder.AppendLine("// -----------------------------| Notes |-----------------------------");
                for (int i = 0; i < notes.Count; i++)
                {
                    dataBuilder.AppendFormat("// {0}. {1}", i + 1, notes[i]).AppendLine();
                }
                dataBuilder.AppendLine("// -------------------------------------------------------------------");
            }
            dataBuilder.Append(source.SourceText);
            return new("cs", dataBuilder.ToString(), Path.GetFileNameWithoutExtension(source.HintName));
        }
    }
}
