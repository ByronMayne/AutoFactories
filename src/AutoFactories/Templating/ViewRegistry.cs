using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoFactories.Templating
{
    internal class ViewRegistry
    {
        private readonly Dictionary<ViewResourceKey, Stream> m_templates;
        private readonly Dictionary<PartialResourceKey, Stream> m_partials;

        public IReadOnlyDictionary<ViewResourceKey, Stream> Views => m_templates;
        public IReadOnlyDictionary<PartialResourceKey, Stream> Partials => m_partials;

        public ViewRegistry()
        {
            m_templates = new Dictionary<ViewResourceKey, Stream>();
            m_partials = new Dictionary<PartialResourceKey, Stream>();
        }

        public void SetView(ViewResourceKey name, Stream stream)
        {
            m_templates[name] = stream;
        }

        public void SetPartial(PartialResourceKey name, Stream stream)
        {
            m_partials[name] =  stream;
        }
    }
}
