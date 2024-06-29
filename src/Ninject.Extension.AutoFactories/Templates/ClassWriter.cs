using System.Text;

namespace Ninject.AutoFactories.Templates
{
    internal class ClassWriter
    {
        private class Scope : IDisposable
        {
            private bool m_isDisposed;
            private readonly bool m_prependSemicolon;
            private readonly ClassWriter m_writer;

            public Scope(ClassWriter writer, bool prependSemicolon)
            {
                m_isDisposed = false;
                m_writer = writer;
                _ = m_writer.WriteLine("{");
                m_writer.m_scope++;
                m_prependSemicolon = prependSemicolon;
            }

            public void Dispose()
            {
                if (m_isDisposed)
                {
                    throw new ObjectDisposedException($"{nameof(ClassWriter)}.{nameof(Scope)}");
                }
                m_isDisposed = true;
                m_writer.m_scope--;

                _ = m_writer.Write("}");
                if (m_prependSemicolon)
                {
                    _ = m_writer.Write(";");
                }

                _ = m_writer.WriteNewLine();
            }
        }

        private readonly char m_inidentChar;
        private readonly int m_initialScope;
        private readonly int m_indentSize;
        private int m_scope;
        private bool m_isPendingIndent;
        private bool m_isPendingNewline;

        private readonly StringBuilder m_builder;

        public ClassWriter(
            int initialScope,
            int indentSize = 4,
            char indentChar = ' ')
        {
            m_inidentChar = indentChar;
            m_initialScope = initialScope;
            m_indentSize = indentSize;
            m_scope = 0;
            m_builder = new StringBuilder();
            m_isPendingIndent = false;
            m_isPendingNewline = false;
        }


        /// <summary>
        /// Writes the content to the class
        /// </summary>
        public ClassWriter Write(string content)
        {
            if (m_isPendingNewline)
            {
                m_isPendingNewline = false;
                AppendNewline();
            }

            if (m_isPendingIndent)
            {
                m_isPendingIndent = false;
                AppendIndent();
            }

            _ = m_builder.Append(content);

            return this;
        }

        /// <summary>
        /// Writes the content to the class only if the condition is true.
        /// </summary>
        /// <param name="condiation"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ClassWriter WriteIf(bool condiation, string content)
        {
            if (!condiation)
            {
                return this;
            }

            _ = Write(content);
            return this;
        }

        public ClassWriter WriteLine(string content)
        {
            _ = Write(content);
            _ = WriteNewLine();
            return this;
        }

        /// <summary>
        /// Writes a block of next and splits the new lines chars 
        /// and gives them the proper indent
        /// </summary>
        /// <param name="input">The block to write</param>
        /// <returns></returns>
        public ClassWriter WriteBlock(string input)
        {
            string[] lines = input.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                _ = WriteLine(line);
            }
            return this;
        }

        public ClassWriter WriteNewLine()
        {
            m_isPendingNewline = true;
            m_isPendingIndent = true;
            return this;
        }

        /// <summary>
        /// Appends the current indent to the writer
        /// </summary>
        private void AppendIndent()
        {
            int indentCount = (m_initialScope + m_scope) * m_indentSize;
            string indent = new(m_inidentChar, indentCount);
            _ = m_builder.Append(indent);
        }

        private void AppendNewline()
        {
            _ = m_builder.AppendLine();
        }


        public IDisposable StartScope(bool appendSemicolon = false)
        {
            return new Scope(this, appendSemicolon);
        }

        public override string ToString()
        {
            return m_builder.ToString();
        }
    }

}
