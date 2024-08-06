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

        static AutoFactoriesGenerator()
        {
            s_options = new Options();
        }

        public AutoFactoriesGenerator() : base($"AutoFactories")
        {
        }

        public override void OnInitialize(SgfInitializationContext context)
        {
            var provider =
                context.AnalyzerConfigOptionsProvider
                    .Combine(context.SyntaxProvider.ForAttributeWithMetadataName(
                          s_options.ClassAttributeType.QualifiedName,
                          predicate: FilterNodes,
                          transform: TransformNodes)
                 .Where(t => t is not null)
                 .Collect());

            context.RegisterPostInitializationOutput(AddSource);
            context.RegisterSourceOutput(provider, (context, tuple) => GenerateFactories(context, tuple.Left, tuple.Right!));
        }


        private static bool IsHandlebarsText(AdditionalText additionalText)
        {
            string extension = Path.GetExtension(additionalText.Path);
            return string.Equals(".hbs", extension, StringComparison.OrdinalIgnoreCase);
        }


        private void AddSource(IncrementalGeneratorPostInitializationContext context)
        {
            Options options = new Options();

            IViewRenderer renderer = new ViewRendererBuilder(options)
                .WriteTo(context.AddSource)
                .LoadModule<CoreViewsModule>()
                .Build();


            renderer.WriteFile(
                $"{options.ClassAttributeType.QualifiedName}.g.cs",
                TemplateName.ClassAttribute, new GenericView()
                {
                    AccessModifier = options.AttributeAccessModifier,
                    Type = options.ClassAttributeType
                });


            renderer.WriteFile(
                $"{options.ParameterAttributeType.QualifiedName}.g.cs",
                TemplateName.ParameterAttribute, new GenericView()
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
            AnalyzerConfigOptionsProvider configOptions,
            ImmutableArray<ClassDeclartionVisitor> visitors)
        {
            Options options = new Options(configOptions);

            IViewRenderer renderer = new ViewRendererBuilder(options)
                .WriteTo(context.AddSource)
                .AddPartialTemplateResolver()
                .LoadModule<CoreViewsModule>()
                .Build();

            foreach (FactoryView view in FactoryDeclartion.Create(visitors).Select(FactoryDeclartion.Map))
            {
                try
                {
                    renderer.WriteFile($"{view.Type.QualifiedName}.g.cs", TemplateName.Factory, view);
                    renderer.WriteFile($"I{view.Type.QualifiedName}.g.cs", TemplateName.FactoryInterface, view);
                }
                catch (HandlebarsRuntimeException runtimeException)
                {
                    Console.WriteLine("D");
                }
            }
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
