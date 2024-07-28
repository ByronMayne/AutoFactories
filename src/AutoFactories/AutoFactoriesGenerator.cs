using AutoFactories.Types;
using AutoFactories.Views;
using AutoFactories.Views.Models;
using AutoFactories.Visitors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories;
using SGF;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using static AutoFactories.Views.View;

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
            IncrementalValueProvider<(AnalyzerConfigOptionsProvider Left, ImmutableArray<ClassDeclartionVisitor?> Right)> provider = context.AnalyzerConfigOptionsProvider
                 .Combine(context.SyntaxProvider.ForAttributeWithMetadataName(
                      s_options.ClassAttributeType.QualifiedName,
                      predicate: FilterNodes,
                      transform: TransformNodes)
                 .Where(t => t is not null)
                 .Collect());

            context.RegisterPostInitializationOutput(AddSource);
            context.RegisterSourceOutput(provider, (context, tuple) => GenerateFactories(context, tuple.Left, tuple.Right!));
        }

        private void AddSource(IncrementalGeneratorPostInitializationContext context)
        {
            Options options = new Options();

            GenericView.AddSource(context.AddSource, "ClassAttribute.hbs", options, o =>
            {
                o.AccessModifier = options.AttributeAccessModifier;
                o.Type = options.ClassAttributeType;
            });

            GenericView.AddSource(context.AddSource, "ParameterAttribute.hbs", options, o =>
            {
                o.AccessModifier = options.AttributeAccessModifier;
                o.Type = options.ParameterAttributeType;
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

            foreach (FactoryView view in FactoryDeclartion.Create(visitors)
                .Select(f => FactoryDeclartion.Map(f, options)))
            {
                view.AddSource(context.AddSource);
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
