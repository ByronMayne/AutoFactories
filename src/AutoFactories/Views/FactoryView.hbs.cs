using AutoFactories.Types;
using AutoFactories.Views.Models;
using Ninject.AutoFactories;
using System;
using System.Collections.Generic;

namespace AutoFactories.Views
{
    internal class FactoryView : View
    {

        public MetadataTypeName Type { get; set; }
        public AccessModifier AccessModifier { get; set; }

        public IList<MethodModel> Methods { get; set; }

        public IList<ParameterModel> Parameters { get; set; }

        public override string HintName => $"{Type.QualifiedName}.g.cs";

        public FactoryView(Options options) : base("UNSET", options)
        {
            Methods = new List<MethodModel>();
            Parameters = new List<ParameterModel>();
        }

        public override void AddSource(AddSourceDelegate addSource)
        {
            string source = Transform("FactoryView.hbs");
            string interfaceSource = Transform("FactoryInterfaceView.hbs");

            addSource($"{Type.QualifiedName}.g.cs", source);
            addSource($"I{Type.QualifiedName}.g.cs", interfaceSource);
        }

        public static void AdddSource(
            AddSourceDelegate addSource, 
            Options options, 
            Action<FactoryView> configure)
        {

            if (configure is null) throw new ArgumentNullException(nameof(configure));

            FactoryView view = new FactoryView(options);
            configure(view);
            view.AddSource(addSource);
        }
    }
}
