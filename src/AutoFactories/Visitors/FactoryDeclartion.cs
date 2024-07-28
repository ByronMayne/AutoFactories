using AutoFactories.Types;
using AutoFactories.Views;
using AutoFactories.Views.Models;
using Ninject.AutoFactories;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Visitors
{

    internal class FactoryDeclartion
    {
        public MetadataTypeName Type { get; }
        public AccessModifier AccessModifier { get; }
        public IReadOnlyList<ClassDeclartionVisitor> Classes { get; }


        private FactoryDeclartion(MetadataTypeName type, IEnumerable<ClassDeclartionVisitor> classes)
        {
            Type = type;
            Classes = classes.ToList();
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


        public static FactoryView Map(FactoryDeclartion declartion, Options options)
            => new FactoryView(options)
            {
                Type = declartion.Type,
                AccessModifier = declartion.AccessModifier,
                Methods = declartion.Classes
                    .SelectMany(c => c.Constructors)
                    .Select(MethodModel.Map)
                    .ToList()
            };
    }
}
