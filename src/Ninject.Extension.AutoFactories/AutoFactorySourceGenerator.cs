using Boxed.Mapping;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ninject.AutoFactory.Mapping;
using Ninject.AutoFactory.Models;
using Ninject.AutoFactory.Templates;
using Ninject.Extension.AutoFactories.Templates;
using SGF;
using System.Collections.Immutable;

namespace Ninject.AutoFactory
{

    [SgfGenerator]
    internal class AutoFactorySourceGenerator : IncrementalGenerator
    {
        private readonly IMapper<ClassDeclarationSyntax, FactoryModel> m_modelMapper;

        public AutoFactorySourceGenerator() : base("AutoFactory")
        {
            m_modelMapper = new FactoryMapper();
        }

        public override void OnInitialize(SgfInitializationContext context)
        {
            // Add build int types 
            context.RegisterPostInitializationOutput(new FromFactoryAttributeTemplate().AddSource);
            context.RegisterPostInitializationOutput(new GenerateFactoryAttributeTemplate().AddSource);
            context.RegisterPostInitializationOutput(new KernalFactoryExtensionsTemplate().AddSource); 

            var factoriesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Ninject.GenerateFactoryAttribute",
                predicate: FilterNodes,
                transform: TransformNodes)
                .Where(t => t is not null);

            var factoryNamesProvider = factoriesProvider
                .Collect();


            //context.RegisterSourceOutput(factoryNames,

            context.RegisterSourceOutput(factoryNamesProvider, GenerateNinjectModule!);
            context.RegisterSourceOutput(factoriesProvider, GenerateFactories!); // Not null because of `is not null`
        }

        private void GenerateNinjectModule(SgfSourceProductionContext context, ImmutableArray<FactoryModel> models)
        {
            new NinjectModuleTemplate(models).AddSource(context);
        }

        private void GenerateFactories(SgfSourceProductionContext context, FactoryModel args)
        {
            new GeneratorTemplate(args).AddSource(context);
        }

        private FactoryModel? TransformNodes(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
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

            FactoryModel model = m_modelMapper.Map(classDeclaration);

            return model;
        }



        private static bool FilterNodes(SyntaxNode node, CancellationToken token)
            => node is ClassDeclarationSyntax classDeclaration &&
                classDeclaration.AttributeLists.Count > 0 &&
                classDeclaration.Modifiers.Any(m => !m.IsKind(SyntaxKind.AbstractKeyword));
    }
}
