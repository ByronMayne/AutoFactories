using AutoFactories.Types;
using HandlebarsDotNet;
using Ninject.AutoFactories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AutoFactories.Views
{
    internal abstract class View
    {
        public delegate void AddSourceDelegate(string hintName, string source);

        private static readonly Assembly s_assembly;
        private readonly IHandlebars m_handlebars;
        private readonly string m_resourceName;

        static View()
        {
            s_assembly = typeof(View).Assembly;
        }

        /// <summary>
        /// Gets the hint name for the generated model
        /// </summary>
        public abstract string HintName { get; }

        public View(string resourceName)
        {
            m_resourceName = resourceName;
            HandlebarsConfiguration configuration = new HandlebarsConfiguration();
            m_handlebars = Handlebars.Create(configuration);
        }

        /// <summary>
        /// Transforms the template into a text version of the class
        /// </summary>
        public string Transform()
        {
            using (Stream stream = s_assembly.GetManifestResourceStream(m_resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string template = reader.ReadToEnd();
                HandlebarsTemplate<object, object> handlebars = m_handlebars.Compile(template);
                return handlebars(this);
            }
        }



        public void AddSource(AddSourceDelegate addSource)
        {
            string source = Transform();
            addSource(HintName, source);
        }
    }
}
