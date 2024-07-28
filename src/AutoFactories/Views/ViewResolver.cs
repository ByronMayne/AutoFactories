using HandlebarsDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AutoFactories.Views
{
    internal class ViewResolver : IPartialTemplateResolver
    {
        private readonly static Assembly s_assembly;
        private readonly static string[] s_resources;
        private readonly static IDictionary<string, string> s_defaultPartials;
        private readonly static Regex s_partialPattern;
        private readonly Options m_options;
        private readonly IDictionary<string, string> m_partialMap;

        static ViewResolver()
        {
            s_assembly = typeof(ViewResolver).Assembly;
            s_resources = s_assembly.GetManifestResourceNames();
            s_partialPattern = new Regex(@"Partials[\\/](?<Name>[^\.]*).hbs", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            Dictionary<string, string> defaultPartials = new Dictionary<string, string>();
            s_defaultPartials = defaultPartials;
            foreach (string resource in s_resources)
            {
                Match match = s_partialPattern.Match(resource);
                if (match.Success)
                {
                    using (Stream stream = s_assembly.GetManifestResourceStream(resource))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string partialName = match.Groups["Name"].Value;
                        defaultPartials[partialName] = reader.ReadToEnd();
                    }
                }
            }
        }

        public ViewResolver(Options options)
        {
            m_options = options;
            m_partialMap = new Dictionary<string, string>(s_defaultPartials);
        }

        public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
        {
            if(m_partialMap.TryGetValue(partialName, out string content))
            {
                env.RegisterTemplate(partialName, content);
                return true;
            }
            return false;
        }
    }
}
