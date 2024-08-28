using AutoFactories.Templating;
using AutoFactories.Views;
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

    [SgfGenerator]
    public class AutoFactoriesGenerator : IncrementalGenerator
    {
        private static Options s_options;

        /// <summary>
        /// Events to subscribe to do handle exceptions that are thrown
        /// </summary>
        public event Action<Exception>? ExceptionHandler;

        static AutoFactoriesGenerator()
        {
            s_options = new Options();
        }

        public AutoFactoriesGenerator() : base($"AutoFactories")
        {
            ExceptionHandler = null;
        }

        public override void OnInitialize(SgfInitializationContext context)
        {
            var provider =
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
                GenerateFactories(context, tuple.Left, tuple.Right.Left, tuple.Right.Right!)
            );
        }


        private static bool IsHandlebarsText(AdditionalText additionalText)
        {
            string extension = Path.GetExtension(additionalText.Path);
            return string.Equals(".hbs", extension, StringComparison.OrdinalIgnoreCase);
        }

        public override void OnException(Exception exception)
        {
            ExceptionHandler?.Invoke(exception);
            base.OnException(exception);
        }

        private void AddSource(IncrementalGeneratorPostInitializationContext context)
        {
            Options options = new Options();

            IViewRenderer renderer = NewViewBuilder(options)
                .LoadEmbeddedTemplates()
                .WriteTo(context.AddSource)
                .Build();


            renderer.WriteFile(
                $"{options.ClassAttributeType.QualifiedName}.g.cs",
                ViewResourceKey.ClassAttribute, new GenericView()
                {
                    AccessModifier = options.AttributeAccessModifier,
                    Type = options.ClassAttributeType
                });


            renderer.WriteFile(
                $"{options.ParameterAttributeType.QualifiedName}.g.cs",
                ViewResourceKey.ParameterAttribute, new GenericView()
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
            ImmutableArray<AdditionalText> additionalTexts,
            AnalyzerConfigOptionsProvider configOptions,
            ImmutableArray<ClassDeclartionVisitor> visitors)
        {
            Options options = new Options(configOptions);

            IViewRenderer renderer = NewViewBuilder(options)
                .WriteTo(context.AddSource)
                .AddPartialTemplateResolver()
                .AddAdditionalTexts(additionalTexts)
                .Build();

            foreach (FactoryView view in FactoryDeclartion.Create(visitors).Select(FactoryDeclartion.Map))
            {
                renderer.WriteFile($"{view.Type.QualifiedName}.g.cs", ViewResourceKey.Factory, view);
                renderer.WriteFile($"I{view.Type.QualifiedName}.g.cs", ViewResourceKey.FactoryInterface, view);
            }
        }

        /// <summary>
        /// Creates a new view renderer builder 
        /// </summary>
        private ViewRendererBuilder NewViewBuilder(Options? options = null)
        {
            ViewRendererBuilder builder = new ViewRendererBuilder();
            if(options is not null) builder.UseOptions(options);
            return builder;
        }

        /// <summary>
        /// Take in the nodes and transform them into vistiors which can be cached
        /// </summary>
        private static ClassDeclartionVisitor? TransformNodes(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            ClassDeclarationSyntax classDeclarationSyntax = (ClassDeclarationSyntax)context.TargetNode;

            ClassDeclartionVisitor visitor = new(false, s_options, context.SemanticModel);

            visitor.VisitClassDeclaration(classDeclarationSyntax);

            return visitor;
        }

        /// <summary>
        /// Filteres the source generator to only look at <see cref="ClassDeclarationSyntax"/> types
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
