using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoFactories.Templating
{
    internal class ViewRegistry
    {
        private readonly List<KeyValuePair<TemplateName, Stream>> m_templates;
        private readonly List<KeyValuePair<PartialName, Stream>> m_partials;

        public IReadOnlyList<KeyValuePair<TemplateName, Stream>> Templates => m_templates;
        public IReadOnlyList<KeyValuePair<PartialName, Stream>> Partials => m_partials;

        public ViewRegistry()
        {
            m_templates = new List<KeyValuePair<TemplateName, Stream>>();
            m_partials = new List<KeyValuePair<PartialName, Stream>>();
        }

        public void SetView(TemplateName name, Stream stream)
        {
            m_templates.Add(new KeyValuePair<TemplateName, Stream>(name, stream));
        }

        public void SetPartial(PartialName name, Stream stream)
        {
            m_partials.Add(new KeyValuePair<PartialName, Stream>(name, stream));
        }
    }
}
