using AutoFactories.Types;
using HandlebarsDotNet;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;
using AutoFactories.Templating;
using AutoFactories.Models;

namespace AutoFactories.Visitors
{

    internal class FactoryDeclaration
    {
        public MetadataTypeName Type { get; }
        public AccessModifier ImplementationAccessModifier { get; }
        public AccessModifier InterfaceAccessModifier { get; }
        public IReadOnlyList<ClassDeclarationVisitor> Classes { get; }
        public IReadOnlyList<ParameterSyntaxVisitor> Parameters { get; }


        private FactoryDeclaration(MetadataTypeName type, IEnumerable<ClassDeclarationVisitor> classes)
        {
            Type = type;
            Classes = classes.ToList();
            Parameters = Classes
                .SelectMany(c => c.Constructors)
                .SelectMany(c => c.Parameters)
                .Where(p => p.HasMarkerAttribute)
                .ToList();

            InterfaceAccessModifier = AccessModifier.MostRestrictive(
                Classes
                .Select(c => c.InterfaceAccessModifier)
                .ToArray());

            ImplementationAccessModifier = AccessModifier.MostRestrictive(
                Classes
                .Select(c => c.FactoryAcessModifier)
                .ToArray());
        }

        public static IEnumerable<FactoryDeclaration> Create(IEnumerable<ClassDeclarationVisitor> classes)
        {
            foreach (IGrouping<MetadataTypeName, ClassDeclarationVisitor> grouping in classes.GroupBy(v => v.FactoryType))
            {
                yield return new FactoryDeclaration(grouping.Key, grouping);
            }
        }


        public static FactoryView Map(FactoryDeclaration declartion)
            => new FactoryView()
            {
                Type = declartion.Type,
                ImplementationAccessModifier = declartion.ImplementationAccessModifier,
                InterfaceAccessModifier = declartion.InterfaceAccessModifier,
                Parameters = declartion.Parameters
                    .Select(ParameterModel.Map)
                    .ToList(),
                Methods = declartion.Classes
                    .SelectMany(c => c.Constructors)
                    .Select(MethodModel.Map)
                    .ToList()
            };
    }
}
