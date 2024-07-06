using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Ninject.AutoFactories.Mapping;
using Ninject.AutoFactories.Models;
using Ninject.AutoFactories.Templates;
using SGF;
using System.Collections.Immutable;

namespace Ninject.AutoFactories
{

    [SgfGenerator]
    internal class AutoFactorySourceGenerator : IncrementalGenerator
    {

        public AutoFactorySourceGenerator() : base("AutoFactory")
        {
        }

        public override void OnInitialize(SgfInitializationContext context)
        {
            // Add build int types 
            context.RegisterPostInitializationOutput(new FromFactoryAttributeTemplate().AddSource);
            context.RegisterPostInitializationOutput(new GenerateFactoryAttributeTemplate().AddSource);
            context.RegisterPostInitializationOutput(new KernalFactoryExtensionsTemplate().AddSource);

            var compilationProvider = context.CompilationProvider;
            var syntaxProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                GeneratorSettings.ClassAttribute.Type.FullName,
                predicate: FilterNodes,
                transform: TransformNodes)
                .Where(t => t is not null)
                .Collect();

            context.RegisterSourceOutput(compilationProvider.Combine(syntaxProvider), (context, args) => Generate(context, args.Left, args.Right!));
        }

        private void Generate(SgfSourceProductionContext context, 
            Compilation compilation,
            ImmutableArray<ProductModel> models)
        {
            List<FactoryModel> factories = FactoryModel.Group(models)
                .ToList();

            new NinjectModuleTemplate(compilation.AssemblyName!, factories).AddSource(context);


            foreach (FactoryModel factoryModel in factories)
            {
                new FactoryTemplate(factoryModel).AddSource(context);
            }
        }

        private ProductModel? TransformNodes(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.TargetNode;
            _ = classDeclaration
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .ToArray();

            ProductMapper mapper = new ProductMapper(context.SemanticModel);



            ProductModel model = mapper.Map(classDeclaration);

            return model;
        }



        private static bool FilterNodes(SyntaxNode node, CancellationToken token)
        {
            return node is ClassDeclarationSyntax classDeclaration &&
                        classDeclaration.AttributeLists.Count > 0 &&
                        classDeclaration.Modifiers.Any(m => !m.IsKind(SyntaxKind.AbstractKeyword));
        }
    }
}
