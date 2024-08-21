using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AutoFactories.Templating
{
    public delegate void AddSourceDelegate(string hintName, string source);

    internal interface IViewRenderer
    {
        string Render<T>(TemplateName templateName, T data);
        void WriteFile<T>(string hintName, TemplateName templateName, Action<T> configure) where T : new();
        void WriteFile<T>(string hintName, TemplateName templateName, T data);
    }

    internal class ViewRenderer : IViewRenderer
    {

        private readonly IHandlebars m_handlebars;
        private readonly ViewRegistry m_viewRegistry;
        private readonly AddSourceDelegate m_addSource;
        private readonly Dictionary<string, HandlebarsTemplate<object, object>> m_contentCache;

        public ViewRenderer(
            AddSourceDelegate addSource,
            IHandlebars handlebars,
            IEnumerable<ViewModule> modules)
        {
            m_addSource = addSource; ;
            m_handlebars = handlebars;
            m_viewRegistry = new ViewRegistry();
            m_contentCache = new Dictionary<string, HandlebarsTemplate<object, object>>();

            foreach (ViewModule module in modules)
            {
                module.Initialize(m_viewRegistry);
            }

            foreach (KeyValuePair<PartialName, Stream> partial in m_viewRegistry.Partials)
            {
                using (StreamReader reader = new StreamReader(partial.Value))
                {
                    string content = reader.ReadToEnd();
                    m_handlebars.RegisterTemplate(partial.Key.Value, content);
                }
            }
        }

        public void WriteFile<T>(string hintName, TemplateName templateName, Action<T> configure) where T : new()
        {
            T data = new T();
            configure(data);
            string source = Render(templateName, data);
            m_addSource(hintName, source);
        }

        public void WriteFile<T>(string hintName, TemplateName templateName, T data)
        {
            string[] items;

       
            string source = Render(templateName, data);
            m_addSource(hintName, source);
        }

        public string Render<T>(TemplateName templateName, T data)
        {
            foreach (KeyValuePair<TemplateName, Stream> entry in m_viewRegistry.Templates)
            {
                if (templateName.Equals(entry.Key))
                {
                    if (!m_contentCache.TryGetValue(templateName.Value, out HandlebarsTemplate<object, object> handlebars))
                    {
                        Stream stream = entry.Value;
                        stream.Position = 0;

                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string content = reader.ReadToEnd();
                            handlebars = m_handlebars.Compile(content);
                            m_contentCache[templateName.Value] = handlebars;
                        }
                    }
                    return handlebars(data!);
                }
            }
            return $"// Template '{templateName}' not found";
        }
    }
}
