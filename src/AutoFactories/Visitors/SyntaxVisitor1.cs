using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace AutoFactories.Visitors
{
    internal class SyntaxVisitor
    {
        private bool m_hasDiagnosticError;
        private readonly List<SyntaxVisitor> m_children;
        private readonly List<Diagnostic> m_diagnostics;

        /// <summary>  
        /// Gets a value indicating whether this visitor or any of its children has a diagnostic error.  
        /// </summary>  
        public bool HasDiagnosticError
        {
            get
            {
                if (m_hasDiagnosticError)
                {
                    return true;
                }
                return m_children.Any(c => c.HasDiagnosticError);
            }
        }

        protected SyntaxVisitor()
        {
            m_children = new List<SyntaxVisitor>();
            m_diagnostics = new List<Diagnostic>();
        }

        /// <summary>
        /// Adds a new child visitor to this visitor. This is used to create a tree of visitors.
        /// </summary>
        /// <param name="child">The child to add</param>
        protected void AddChild(SyntaxVisitor child)
        {
            m_children.Add(child);
        }

        /// <summary>  
        /// Adds a diagnostic to this visitor.  
        /// </summary>  
        /// <param name="diagnostic">The diagnostic to add.</param>  
        public void AddDiagnostic(Diagnostic diagnostic)
        {
            m_hasDiagnosticError |= diagnostic.Severity == DiagnosticSeverity.Error;
            m_diagnostics.Add(diagnostic);
        }

        /// <summary>  
        /// Gets all diagnostics from this visitor and its children.  
        /// </summary>  
        /// <returns>An enumerable of diagnostics.</returns>  
        public IEnumerable<Diagnostic> GetDiagnostics()
        {
            foreach (Diagnostic diagnostic in m_diagnostics)
            {
                yield return diagnostic;
            }

            foreach (SyntaxVisitor child in m_children)
            {
                foreach (Diagnostic diagnostic in child.GetDiagnostics())
                {
                    yield return diagnostic;
                }
            }
        }
    }
}