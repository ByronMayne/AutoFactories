using AutoFactories.Types;
using AutoFactories.Views;
using AutoFactories.Views.Models;
using HandlebarsDotNet;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;
using AutoFactories.Templating;

namespace AutoFactories.Visitors
{

    internal class FactoryDeclartion
    {
        public MetadataTypeName Type { get; }
        public AccessModifier AccessModifier { get; }
        public IReadOnlyList<ClassDeclartionVisitor> Classes { get; }
        public IReadOnlyList<ParameterSyntaxVisitor> Parameters { get; }


        private FactoryDeclartion(MetadataTypeName type, IEnumerable<ClassDeclartionVisitor> classes)
        {
            Type = type;
            Classes = classes.ToList();
            Parameters = Classes
                .SelectMany(c => c.Constructors)
                .SelectMany(c => c.Parameters)
                .Where(p => p.HasMarkerAttribute)
                .ToList();

            AccessModifier = Classes.Any(c => c.AccessModifier != AccessModifier.Public)
                    ? AccessModifier.Internal
                    : AccessModifier.Public;
        }

        public static IEnumerable<FactoryDeclartion> Create(IEnumerable<ClassDeclartionVisitor> classes)
        {
            foreach (IGrouping<MetadataTypeName, ClassDeclartionVisitor> grouping in classes.GroupBy(v => v.FactoryType))
            {
                yield return new FactoryDeclartion(grouping.Key, grouping);
            }
        }


        public static FactoryView Map(FactoryDeclartion declartion)
            => new FactoryView()
            {
                Type = declartion.Type,
                AccessModifier = declartion.AccessModifier,
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
