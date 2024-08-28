using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoFactories.Templating
{
    internal class ViewRegistry
    {
        private readonly List<KeyValuePair<ViewResourceKey, Stream>> m_templates;
        private readonly List<KeyValuePair<PartialResourceKey, Stream>> m_partials;

        public IReadOnlyList<KeyValuePair<ViewResourceKey, Stream>> Templates => m_templates;
        public IReadOnlyList<KeyValuePair<PartialResourceKey, Stream>> Partials => m_partials;

        public ViewRegistry()
        {
            m_templates = new List<KeyValuePair<ViewResourceKey, Stream>>();
            m_partials = new List<KeyValuePair<PartialResourceKey, Stream>>();
        }

        public void SetView(ViewResourceKey name, Stream stream)
        {
            m_templates.Add(new KeyValuePair<ViewResourceKey, Stream>(name, stream));
        }

        public void SetPartial(PartialResourceKey name, Stream stream)
        {
            m_partials.Add(new KeyValuePair<PartialResourceKey, Stream>(name, stream));
        }
    }
}
