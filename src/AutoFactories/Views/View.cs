using AutoFactories.Templating;
using HandlebarsDotNet;
using System;
using System.IO;
using System.Reflection;

namespace AutoFactories.Views
{

    internal abstract class View
    {
        public delegate void AddSourceDelegate(string hintName, string source);

        private static readonly Assembly s_assembly;
        private readonly IHandlebars m_handlebars;
        protected readonly string m_resourceName;

        static View()
        {
            s_assembly = typeof(View).Assembly;
        }

        /// <summary>
        /// Gets the hint name for the generated model
        /// </summary>
        public abstract string HintName { get; }

        public View(string resourceName, Options options)
        {
            m_resourceName = resourceName;
            m_handlebars = new HandlebarsBuilder(options)
                .AddPartialTemplateResolver()
                .Build();
        }

        /// <summary>
        /// Transforms the template into a text version of the class
        /// </summary>
        protected string Transform(string resourceName)
        {
            using Stream stream = s_assembly.GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                string resourceNames = string.Join("\n - ", s_assembly.GetManifestResourceNames())!;
                throw new ArgumentException($"Unable to find resources named '{resourceName}'. The resources found are:\n {resourceNames}");
            }

            using StreamReader reader = new(stream);
            string template = reader.ReadToEnd();
            HandlebarsTemplate<object, object> handlebars = m_handlebars.Compile(template);
            return handlebars(this);
        }



        public virtual void AddSource(AddSourceDelegate addSource)
        {
            string source = Transform(m_resourceName);
            addSource(HintName, source);
        }

    }
}
