using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;


namespace AutoFactories.Templating
{
    public delegate void AddSourceDelegate(string hintName, string source);

    internal interface IViewRenderer
    {
        string Render<T>(ViewResourceKey templateName, T data);
        void WriteFile<T>(string hintName, ViewResourceKey templateName, Action<T> configure) where T : new();
        void WriteFile<T>(string hintName, ViewResourceKey templateName, T data);
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
            ViewRegistry viewRegistry)
        {
            m_addSource = addSource; ;
            m_handlebars = handlebars;
            m_viewRegistry = viewRegistry;
            m_contentCache = new Dictionary<string, HandlebarsTemplate<object, object>>();
        }

        public void WriteFile<T>(string hintName, ViewResourceKey templateName, Action<T> configure) where T : new()
        {
            T data = new T();
            configure(data);
            string source = Render(templateName, data);
            m_addSource(hintName, source);
        }

        public void WriteFile<T>(string hintName, ViewResourceKey templateName, T data)
        {
            string source = Render(templateName, data);
            m_addSource(hintName, source);
        }

        public string Render<T>(ViewResourceKey templateName, T data)
        {
            if (m_viewRegistry.Views.TryGetValue(templateName, out Stream stream))
            {
                if (!m_contentCache.TryGetValue(templateName.Value, out HandlebarsTemplate<object, object> handlebars))
                {
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();
                        handlebars = m_handlebars.Compile(content);
                        m_contentCache[templateName.Value] = handlebars;
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
                    exceptionBuilder.AppendFormat("#error Unhandled exception while building the template {0}", templateName);
                    foreach (string line in exception.Message.Split('\n'))
                    {
                        exceptionBuilder.AppendFormat("// {0}", line).AppendLine();
                    }
                    return exceptionBuilder.ToString();
                }
            }
            return $"// Template '{templateName}' not found";
        }

        public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
        {
            if (PartialResourceKey.TryFrom(partialName, out var resourceKey))
            {
                if (m_viewRegistry.Partials.TryGetValue(resourceKey, out Stream stream))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();
                        env.RegisterTemplate(partialName, content);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
