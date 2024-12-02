using AutoInterfaceAttributes;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace AutoFactories.Templating
{
    public delegate void AddSourceDelegate(string hintName, string source);

    [AutoInterface]
    internal class ViewRenderer : IViewRenderer
    {
        private readonly IHandlebars m_handlebars;
        private readonly AddSourceDelegate m_addSource;
        private readonly Dictionary<string, HandlebarsTemplate<object, object>> m_contentCache;
        private readonly IReadOnlyDictionary<ViewKey, Stream> m_resourceMap;

        public ViewRenderer(
            AddSourceDelegate addSource,
            IHandlebars handlebars,
            IReadOnlyDictionary<ViewKey, Stream> resourceMap)
        {
            m_addSource = addSource; ;
            m_handlebars = handlebars;
            m_contentCache = new Dictionary<string, HandlebarsTemplate<object, object>>();
            m_resourceMap = resourceMap;
        }

        public void WriteFile<T>(string hintName, ViewKey templateName, Action<T> configure) where T : new()
        {
            T data = new T();
            configure(data);
            string source = Render(templateName, data);
            m_addSource(hintName, source);
        }

        public void WritePage<T>(string hintName, ViewKey page, T data)
        {
            string source = Render(page, data);
            m_addSource(hintName, source);
        }

        public string Render<T>(ViewKey resourceName, T data)
        {
            if (m_resourceMap.TryGetValue(resourceName, out Stream stream))
            {
                if (!m_contentCache.TryGetValue(resourceName.Value, out HandlebarsTemplate<object, object> handlebars))
                {
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();
                        handlebars = m_handlebars.Compile(content);
                        m_contentCache[resourceName.Value] = handlebars;
                    }
                }
                try
                {
                    string renderedTemplate = handlebars(data!);

                    return renderedTemplate;
                }
                catch (Exception exception)
                {
                    StringBuilder exceptionBuilder = new StringBuilder();
                    exceptionBuilder.AppendFormat("#error Unhandled exception while building the template {0}", resourceName);
                    foreach (string line in exception.Message.Split('\n'))
                    {
                        exceptionBuilder.AppendFormat("// {0}", line).AppendLine();
                    }
                    return exceptionBuilder.ToString();
                }
            }
            return $"// Template '{resourceName}' not found";
        }
    }
}
