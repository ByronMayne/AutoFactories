using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactory.Mapping;
using Ninject.AutoFactory.Models;
using Ninject.AutoFactory.Templates;
using Ninject.Extension.AutoFactories;
using Ninject.Extension.AutoFactories.Models;
using Ninject.Extension.AutoFactories.Templates;
using SGF;
using System.Collections.Immutable;

namespace Ninject.AutoFactory
{

    [SgfGenerator]
    internal class AutoFactorySourceGenerator : IncrementalGenerator
    {
        private readonly ProductMapper m_modelMapper;

        public AutoFactorySourceGenerator() : base("AutoFactory")
        {
            m_modelMapper = new ProductMapper();
        }

        public override void OnInitialize(SgfInitializationContext context)
        {
            // Add build int types 
            context.RegisterPostInitializationOutput(new FromFactoryAttributeTemplate().AddSource);
            context.RegisterPostInitializationOutput(new GenerateFactoryAttributeTemplate().AddSource);
            context.RegisterPostInitializationOutput(new KernalFactoryExtensionsTemplate().AddSource);

            var factoriesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                GeneratorSettings.ClassAttribute.FullName,
                predicate: FilterNodes,
                transform: TransformNodes)
                .Where(t => t is not null)
                .Collect();

            //context.RegisterSourceOutput(factoryNames,

            context.RegisterSourceOutput(factoriesProvider, Generate!);
        }

        private void Generate(SgfSourceProductionContext context, ImmutableArray<ProductModel> models)
        {
 

            List<FactoryModel> factories = models
                .GroupBy(m => m.FactoryType)
                .Select(g => new FactoryModel(g.Key)
                {
                    Products = g.ToList()
                })
                .ToList();


            new NinjectModuleTemplate(factories).AddSource(context);


            foreach (FactoryModel factoryModel in factories)
            {
                new FactoryTemplate(factoryModel).AddSource(context);
            }

           // new GeneratorTemplate(args).AddSource(context);
        }

        private ProductModel? TransformNodes(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            ClassDeclarationSyntax classDeclaration = (ClassDeclarationSyntax)context.TargetNode;


            ConstructorDeclarationSyntax[] constructors = classDeclaration
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .ToArray();

            ProductModel model = m_modelMapper.Map((context.SemanticModel, classDeclaration));

            return model;
        }



        private static bool FilterNodes(SyntaxNode node, CancellationToken token)
            => node is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0 &&
                classDeclaration.Modifiers.Any(m => !m.IsKind(SyntaxKind.AbstractKeyword));
    }
}
