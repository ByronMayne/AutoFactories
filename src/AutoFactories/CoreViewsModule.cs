namespace AutoFactories
{
    public class CoreViewsModule : ViewModule
    {
        public CoreViewsModule() : base("Core")
        {
        }

        public override void Load()
        {
            SetTemplate(TemplateName.Factory, "FactoryView.hbs");
            SetTemplate(TemplateName.FactoryInterface, "FactoryInterfaceView.hbs");
            SetTemplate(TemplateName.ClassAttribute, "ClassAttribute.hbs");
            SetTemplate(TemplateName.FactoryInterface, "FactoryInterfaceView.hbs");
            SetTemplate(TemplateName.ParameterAttribute, "ParameterAttribute.hbs");

            SetPartial(PartialName.FactoryProperties, "Partials\\FactoryProperties.hbs");
            SetPartial(PartialName.FactoryConstructor, "Partials\\FactoryConstructors.hbs");
            SetPartial(PartialName.FactoryMethod, "Partials\\FactoryMethod.hbs");
            SetPartial(PartialName.FactoryFields, "Partials\\FactoryFields.hbs");
            SetPartial(PartialName.MethodParameters, "Partials\\Parameters.hbs");
            SetPartial(PartialName.FactoryNamespaces, "Partials\\FactoryNamespaces.hbs");
        }
    }
}
