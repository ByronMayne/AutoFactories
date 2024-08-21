using HandlebarsDotNet;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.PathStructure;
using System;
using System.Collections;
using System.Collections.Generic;



namespace AutoFactories.Templating
{

    internal class ViewRendererBuilder
    {
        private readonly HandlebarsConfiguration m_configuration;
        private readonly List<Action<IHandlebars>> m_setupActions;
        private readonly List<ViewModule> m_modules;
        private readonly ViewRegistry m_viewRegistry;
        private AddSourceDelegate? m_outputTo;
        private Options m_options;

        public ViewRendererBuilder()
        {
            m_options = new Options();
            m_modules = new List<ViewModule>();
            m_viewRegistry = new ViewRegistry();
            m_setupActions = [
             h => h.RegisterHelper("each-if", EachIf)];

            m_configuration = new HandlebarsConfiguration()
            {
                NoEscape = true,
            };

        }

        public ViewRendererBuilder UseOptions(Options options)
        {
            m_options = options;
            return this;
        }

        /// <summary>
        /// Sets the delegate the the source file and code is written too
        /// </summary>
        public ViewRendererBuilder WriteTo(AddSourceDelegate addSource)
        {
            m_outputTo = addSource;
            return this;
        }

        /// <summary>
        /// Adds a nwe resovler that can find partial templates that are contained within this assembly
        /// </summary>
        public ViewRendererBuilder AddPartialTemplateResolver()
        {
            m_configuration.PartialTemplateResolver = new PartialTemplateResolver(m_options);
            return this;
        }

        /// <summary>
        /// Loads all the views from the given module
        /// </summary>
        public ViewRendererBuilder LoadModule<TModule>() where TModule : ViewModule, new()
        {
            LoadModule(new TModule());
            return this;
        }

        /// <summary>
        /// Loads a new module and parses all it's templates
        /// </summary>
        public ViewRendererBuilder LoadModule(ViewModule module)
        {
            m_modules.Add(module);
            return this;
        }

        /// <summary>
        /// Adds templates that can override the built in ones
        /// </summary>
        /// <param name="templates">The templates to resolve</param>
        /// <returns></returns>
        public ViewRendererBuilder LoadModules(IEnumerable<ViewModule> modules)
        {
            m_modules.AddRange(modules);
            return this;
        }

        public IViewRenderer Build()
        {

            IHandlebars handlebars = Handlebars.Create(m_configuration);
            foreach (Action<IHandlebars> setupAction in m_setupActions)
            {
                setupAction(handlebars);
            }
            return new ViewRenderer(m_outputTo, handlebars, m_modules);
        }


        private void EachIf(EncodedTextWriter output, BlockHelperOptions options, Context context, Arguments arguments)
        {

            IEnumerable? items = arguments[0] as IEnumerable;
            object condition = arguments[1];

            if (items is null) return;

            ReflectionMemberAccessor memberAccessor = new ReflectionMemberAccessor(Array.Empty<IMemberAliasProvider>());
            ChainSegment chainSegment = ChainSegment.Create(condition);

            List<object> values = new List<object>();

            foreach (object value in items)
            {
                if (memberAccessor.TryGetValue(value, chainSegment, out object? rawCondition))
                {
                    if (rawCondition is bool conditation && conditation)
                    {
                        values.Add(value);
                    }
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                BindingContext bindingContext = options.CreateFrame(values[i]);
                bindingContext.Data.CreateProperty(ChainSegment.First, i == 0, out _);
                bindingContext.Data.CreateProperty(ChainSegment.Last, i == values.Count - 1, out _);
                bindingContext.Data.CreateProperty(ChainSegment.Index, i == 0, out _);

                options.Template(in output, bindingContext);
            }
        }
    }
}
