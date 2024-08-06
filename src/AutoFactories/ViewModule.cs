using AutoFactories.Templating;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoFactories
{
    public abstract class ViewModule
    {
        protected readonly Assembly m_assembly;
        private readonly string[] m_resourceNames;
        private ViewRegistry? m_viewRegistry;


        public string Name { get; }

        private ViewRegistry viewRegistry
        {
            get
            {
                if (m_viewRegistry is null)
                {
                    throw new InvalidOperationException($"You can only register views and tempalte within the {nameof(Load)} callback");
                }
                return m_viewRegistry;
            }
        }

        protected ViewModule(string name)
        {
            Name = name;
            m_assembly = GetType().Assembly;
            m_resourceNames = m_assembly.GetManifestResourceNames();
        }

        public abstract void Load();

        /// <summary>
        /// Sets an existing pre-defined view to the given value. All of these views already exist in the 
        /// base AutoFactories libraries.
        /// </summary>
        /// <param name="viewKey">The enum name of the view you want to set</param>
        /// <param name="resourcePath">The path to the item in the assembly resources</param>
        protected void SetTemplate(TemplateName viewKey, string resourcePath)
        {
            Stream stream = GetStream(resourcePath);
            viewRegistry.SetView(viewKey, stream);
        }
        
        /// <summary>
        /// Sets an existing pre-defined partial view to the given value. All of these partial views
        /// already exist in the base library.
        /// </summary>
        /// <param name="partialName">The name of the partial view</param>
        /// <param name="resourcePath">The resource path for the view</param>
        protected void SetPartial(PartialName partialName, string resourcePath)
        {
            Stream stream = GetStream(resourcePath);
            viewRegistry.SetPartial(partialName, stream);
        }

        /// <summary>
        /// Calls the load method and returns back the results
        /// </summary>
        internal void Initialize(ViewRegistry registry)
        {
            try
            {
                m_viewRegistry = registry;
                Load();
            }
            finally
            {
                m_viewRegistry = null;
            }
        }

        private Stream GetStream(string resourcePath)
        {
            if (!m_resourceNames.Contains(resourcePath))
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"Resource Not Found: The assembly {m_assembly.FullName} does not contain a resource called {resourcePath}.");
                builder.AppendLine("The contained resources are:");
                foreach (var r in m_resourceNames)
                {
                    builder.AppendLine($" - {r}");
                }
                throw new Exception(builder.ToString());
            }

            using (Stream stream = m_assembly.GetManifestResourceStream(resourcePath))
            {
                MemoryStream memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
