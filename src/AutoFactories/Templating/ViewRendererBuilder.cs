using HandlebarsDotNet;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.PathStructure;
using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;



namespace AutoFactories.Templating
{

    internal class ViewRendererBuilder
    {
        private static readonly Regex s_partialRegex;
        private static readonly Assembly s_assembly;

        private readonly HandlebarsConfiguration m_configuration;
        private readonly List<Action<IHandlebars>> m_setupActions;
        private readonly ViewRegistry m_viewRegistry;
        private AddSourceDelegate? m_outputTo;
        private Options m_options;

        static ViewRendererBuilder()
        {
            s_assembly = typeof(ViewRendererBuilder).Assembly;
            s_partialRegex = new Regex(@"[\\/]partials[\\/].*\.hbs", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public ViewRendererBuilder()
        {
            m_options = new Options();
            m_viewRegistry = new ViewRegistry();
            m_setupActions = [
            h => h.RegisterHelper("each-if", EachIf)];

            m_configuration = new HandlebarsConfiguration()
            {
                NoEscape = true,
            };
        }

        /// <summary>
        /// Loops over the assembly and loads all the templates that 
        /// are defined 
        /// </summary>
        public ViewRendererBuilder LoadEmbeddedTemplates()
        {
            foreach (string resourcePath in s_assembly.GetManifestResourceNames()
                 .Where(p => string.Equals(Path.GetExtension(p), ".hbs")))
            {
                string fileName = Path.GetFileNameWithoutExtension(resourcePath);
                ViewResourceKey templateName = ViewResourceKey.From(fileName);
                Stream stream = s_assembly.GetManifestResourceStream(resourcePath);
                m_viewRegistry.SetView(templateName, stream);
            }
            return this;
        }

        public ViewRendererBuilder AddAdditionalTexts(ImmutableArray<AdditionalText> templates)
        {
            foreach (AdditionalText template in templates)
            {
                string filePath = template.Path;
                string fileName = Path.GetFileName(filePath);
                string? text = template.GetText()?.ToString();

                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    writer.WriteAsync(text);
                }
                stream.Position = 0;

                string name = Path.GetFileNameWithoutExtension(filePath);
                
                if (s_partialRegex.IsMatch(filePath))
                {
                    PartialResourceKey partialName = PartialResourceKey.From(name);
                    m_viewRegistry.SetPartial(partialName, stream);
                }
                else
                {
                    ViewResourceKey templateName = ViewResourceKey.From(name);
                    m_viewRegistry.SetView(templateName, stream);
                }
            }
            return this;
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

        public IViewRenderer Build()
        {

            IHandlebars handlebars = Handlebars.Create(m_configuration);
            foreach (Action<IHandlebars> setupAction in m_setupActions)
            {
                setupAction(handlebars);
            }

            foreach (KeyValuePair<PartialResourceKey, Stream> partial in m_viewRegistry.Partials)
            {
                using (StreamReader reader = new StreamReader(partial.Value))
                {
                    string content = reader.ReadToEnd();
                    handlebars.RegisterTemplate(partial.Key.Value, content);
                }
            }

            return new ViewRenderer(m_outputTo!, handlebars, m_viewRegistry);
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
