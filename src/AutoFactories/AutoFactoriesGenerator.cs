using AutoFactories.CodeAnalysis;
using AutoFactories.Models;
using AutoFactories.Templating;
using AutoFactories.Visitors;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using SGF;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace AutoFactories
{

    [IncrementalGenerator]
    public class AutoFactoriesGenerator : IncrementalGenerator
    {
        private static readonly Options s_options;

        static AutoFactoriesGenerator()
        {
            s_options = new Options();
        }

        public AutoFactoriesGenerator() : base($"AutoFactories")
        {}

        public override void OnInitialize(SgfInitializationContext context)
        {
            IncrementalValueProvider<(ImmutableArray<AdditionalText> Left, (AnalyzerConfigOptionsProvider Left, ImmutableArray<ClassDeclarationVisitor?> Right) Right)> provider =
                context.AdditionalTextsProvider
                    .Where(IsHandlebarsText).Collect().Combine(
                        context.AnalyzerConfigOptionsProvider
                        .Combine(context.SyntaxProvider.ForAttributeWithMetadataName(
                            s_options.ClassAttributeType.QualifiedName,
                            predicate: FilterNodes,
                            transform: TransformNodes)
                        .Where(t => t is not null)
                 .Collect()));

            context.RegisterPostInitializationOutput(AddSource);
            context.RegisterSourceOutput(provider, (context, tuple) =>
            {
                ImmutableArray<ViewResourceText> templateTexts = ProcessTexts(tuple.Left)
                    .ToImmutableArray();

                GenerateFactories(context, templateTexts, tuple.Right.Left, tuple.Right.Right!);
            });
        }

        /// <summary>
        /// Loops over additional texts and converts them to <see cref="ViewResourceText"/> if they are valid.
        /// </summary>
        private static IEnumerable<ViewResourceText> ProcessTexts(IEnumerable<AdditionalText> additionalTexts)
        {
            foreach(var item in  additionalTexts)
            {
                if(ViewResourceText.TryParse(item, out var text))
                {
                    yield return text;
                }
            }
        }

        private static bool IsHandlebarsText(AdditionalText additionalText)
        {
            string extension = Path.GetExtension(additionalText.Path);
            return string.Equals(".hbs", extension, StringComparison.OrdinalIgnoreCase);
        }

        private void AddSource(IncrementalGeneratorPostInitializationContext context)
        {
            Options options = new();

            IViewRenderer renderer = NewViewBuilder(options)
                .LoadEmbeddedTemplates()
                .WriteTo(context.AddSource)
                .Build();

            renderer.WritePage(
                $"{options.ClassAttributeType.QualifiedName}.g.cs",
                ViewKey.ClassAttribute, new GenericView()
                {
                    AccessModifier = options.AttributeAccessModifier,
                    Type = options.ClassAttributeType
                });


            renderer.WritePage(
                $"{options.ParameterAttributeType.QualifiedName}.g.cs",
                ViewKey.ParameterAttribute, new GenericView()
                {
                    AccessModifier = options.AttributeAccessModifier,
                    Type = options.ParameterAttributeType
                });
        }

        /// <summary>
        /// Invoked once for every singlef actory model 
        /// </summary>
        private void GenerateFactories(
            SgfSourceProductionContext context,
            ImmutableArray<ViewResourceText> templateTexts,
            AnalyzerConfigOptionsProvider configOptions,
            ImmutableArray<ClassDeclarationVisitor> visitors)
        {
            Options options = new(configOptions);
            foreach (AdditionalText text in templateTexts)
            {
                Logger.Information($"Include: {text.Path}");
            }
            IViewRenderer renderer = NewViewBuilder(options)
                .WriteTo(context.AddSource)
                .AddTemplateTexts(templateTexts)
                .Build();

            IEnumerable<ClassDeclarationVisitor> validVisitors = visitors
                .Where(visitor => !visitor.GetDiagnostics().Any(v => v.Severity == DiagnosticSeverity.Error));


            List<FactoryViewModel> factories = FactoryDeclaration.Create(validVisitors)
                .Select(FactoryDeclaration.Map)
                .ToList();

            foreach (FactoryViewModel view in factories)
            {
                renderer.WritePage($"{view.Type.QualifiedName}.g.cs", ViewKey.Factory, view);
                renderer.WritePage($"I{view.Type.QualifiedName}.g.cs", ViewKey.FactoryInterface, view);
            }

            GenericViewModel genericModel = new GenericViewModel()
            {
                ["Factories"] = factories
            };

            // Render out all static files 
            foreach (ViewResourceText view in templateTexts.Where(t => t.Kind == ViewKind.Static))
            {
                string fileName = Path.GetFileNameWithoutExtension(view.Path);
                renderer.WritePage($"{fileName}.g.cs", view.Key, genericModel);
            }
        }


        /// <summary>
        /// Creates a new view renderer builder 
        /// </summary>
        private ViewRendererBuilder NewViewBuilder(Options? options = null)
        {
            ViewRendererBuilder builder = new();
            if (options is not null)
            {
                _ = builder.UseOptions(options);
            }

            return builder;
        }

        /// <summary>
        /// Take in the nodes and transform them into vistiors which can be cached
        /// </summary>
        private static ClassDeclarationVisitor? TransformNodes(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.TargetNode;

            ClassDeclarationVisitor visitor = new(false, s_options, context.SemanticModel);

            visitor.VisitClassDeclaration(classDeclarationSyntax);

            return visitor;
        }

        /// <summary>
        /// Filters the source generator to only look at <see cref="ClassDeclarationSyntax"/> types
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <param name="token"><see cref="CancellationToken"/></param>
        /// <returns>True if the node should be processed otherwise false</returns>
        private static bool FilterNodes(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax classDeclaration &&
                        classDeclaration.AttributeLists.Count > 0 &&
                        classDeclaration.Modifiers.Any(m => !m.IsKind(SyntaxKind.AbstractKeyword));
        }
    }
}
