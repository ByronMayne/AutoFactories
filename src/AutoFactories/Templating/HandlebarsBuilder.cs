using HandlebarsDotNet;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.PathStructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;


namespace AutoFactories.Templating
{
    internal class HandlebarsBuilder
    {
        private readonly Options m_options;
        private readonly HandlebarsConfiguration m_configuration;

        public HandlebarsBuilder(Options options)
        {
            m_options = options;
            m_configuration = new HandlebarsConfiguration()
            {
                NoEscape = true,
            };

        }

        /// <summary>
        /// Adds a nwe resovler that can find partial templates that are contained within this assembly
        /// </summary>
        public HandlebarsBuilder AddPartialTemplateResolver()
        {
            m_configuration.PartialTemplateResolver = new PartialTemplateResolver(m_options);
            return this;
        }

        public IHandlebars Build()
        {
            IHandlebars handlebars = Handlebars.Create(m_configuration);
            handlebars.RegisterHelper("each-if", EachIf);
            return handlebars;
        }

   
        private void EachIf(EncodedTextWriter output, BlockHelperOptions options, Context context, Arguments arguments)
        {

            IEnumerable? items = arguments[0] as IEnumerable;
            object condition = arguments[1];

            if (items is null) return;

            ReflectionMemberAccessor memberAccessor = new ReflectionMemberAccessor(Array.Empty<IMemberAliasProvider>());
            ChainSegment chainSegment = ChainSegment.Create(condition);

            List<object> values = new List<object>();

            foreach (object value in items)
            {
                if (memberAccessor.TryGetValue(value, chainSegment, out object? rawCondition))
                {
                    if (rawCondition is bool conditation && conditation)
                    {
                        values.Add(value);
                    }
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                BindingContext bindingContext = options.CreateFrame(values[i]);
                bindingContext.Data.CreateProperty(ChainSegment.First, i == 0, out _);
                bindingContext.Data.CreateProperty(ChainSegment.Last, i == values.Count - 1, out _);
                bindingContext.Data.CreateProperty(ChainSegment.Index, i == 0, out _);

                options.Template(in output, bindingContext);
            }
        }
    }
}
