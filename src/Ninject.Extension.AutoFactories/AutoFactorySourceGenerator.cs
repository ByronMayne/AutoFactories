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

namespace Ninject.Extension.AutoFactories
{

    [SgfGenerator]
    internal class AutoFactorySourceGenerator : IncrementalGenerator
    {
        private readonly FactoryMapper m_modelMapper;

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

            IncrementalValueProvider<ImmutableArray<FactoryModel?>> factoriesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                GeneratorSettings.ClassAttribute.FullName,
                predicate: FilterNodes,
                transform: TransformNodes)
                .Where(t => t is not null)
                .Collect();

            context.RegisterSourceOutput(factoriesProvider, GenerateNinjectModule!);
        }

        private void GenerateNinjectModule(SgfSourceProductionContext context, ImmutableArray<FactoryModel> models)
        {
            // Generate ninject module that contains all the types
            new NinjectModuleTemplate(models).AddSource(context);

            foreach (FactoryModel model in models.Distinct())
            {
                new GeneratorTemplate(model).AddSource(context);
            }
        }

        private FactoryModel? TransformNodes(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
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

            FactoryModel model = m_modelMapper.Map((context.SemanticModel, classDeclaration));
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
