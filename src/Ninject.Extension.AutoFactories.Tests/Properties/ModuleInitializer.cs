using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;
using VerifyTests;

namespace Ninject.AutoFactories.Properties;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
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
