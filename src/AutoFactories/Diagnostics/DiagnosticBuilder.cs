using Microsoft.CodeAnalysis;
using System;

namespace AutoFactories.Diagnostics
{

    internal abstract class DiagnosticBuilder
    {
        private readonly Lazy<DiagnosticDescriptor> m_descriptor;

        /// <summary>
        /// Gets the descriptor for the given diagnostics
        /// </summary>
        public DiagnosticDescriptor Descriptor => m_descriptor.Value;

        /// <summary>
        /// Gets the uniqu id for the descriptr
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the title of the diagnostics
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Gets the message format 
        /// </summary>
        public string MessageFormat { get; protected set; }

        /// <summary>
        /// Gets the category
        /// </summary>
        public string Category { get; protected set; }

        /// <summary>
        /// Gets the description
        /// </summary>
        public string? Description { get; protected set; }

        /// <summary>
        /// Gets the severity
        /// </summary>
        public DiagnosticSeverity Severity { get; protected set; }

        protected DiagnosticBuilder(int id, string title, string category, string messageFormat)
        {
            Id = $"AF{id:000}";
            Title = title;
            MessageFormat = messageFormat;
            Category = category;
            Severity = DiagnosticSeverity.Error;
            m_descriptor = new Lazy<DiagnosticDescriptor>(CreateDescriptor);

        }

        /// <summary>
        /// Lazy implemention that creates the descriptor
        /// </summary>
        /// <returns></returns>
        protected virtual DiagnosticDescriptor CreateDescriptor()
            => new DiagnosticDescriptor(Id, Title, MessageFormat, Category, Severity, true, Description);
    }
}
