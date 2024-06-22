using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using Ninject.AutoFactory.Models;

namespace Ninject.AutoFactory.Templates
{
    internal static class Iterator
    {
        public record struct Item<T>(T Value, bool IsFirst, bool IsLast)
        {
            public static implicit operator T(Item<T> item)
                => item.Value;
        }

        public static IEnumerable<Item<T>> Create<T>(IEnumerable<T> enumerable)
        {
            T[] collection = enumerable as T[] ?? enumerable.ToArray();
            int count = collection.Length;

            for (int i = 0; i < count; i++)
            {
                yield return new Item<T>(collection[i], i == 0, i + 1 >= count);
            }
        }
    }

    internal class GeneratorTemplate : Template
    {
        private readonly FactoryModel m_model;

        public GeneratorTemplate(FactoryModel model) : base($"{model.Namespace}.{model.TypeName}.g.cs")
        {
            m_model = model;
        }

        /// <inheritdoc cref="Template"/>
        /// <see cref=""/>
        protected override string Render()
        {
            return $$"""
                using System;
                using Ninject.Syntax;
                using Ninject.Parameters;
                
                namespace {{m_model.Namespace}}
                {
                    /// <summary>
                    /// Defines a contract for creating instances of <see cref="{{m_model.Namespace}}.{{m_model.TypeName}}"/>
                    /// </summary>
                    {{m_model.InterfaceAccessModifier}} interface {{m_model.FactoryInterfaceTypeName}} 
                    {
                        {{RenderMethods(true)}}
                    }

                    /// <summary>
                    /// Defines a contract for creating instances of <see cref="{{m_model.Namespace}}.{{m_model.TypeName}}"/>
                    /// </summary>
                    {{m_model.ClassAccessModifier}} sealed partial class {{m_model.FactoryTypeName}}: {{m_model.FactoryInterfaceTypeName}} 
                    {
                        private readonly IResolutionRoot m_resolutionRoot;

                        public {{m_model.TypeName}}Factory(IResolutionRoot resolutionRoot)
                        {
                            m_resolutionRoot = resolutionRoot;
                        }

                        {{RenderMethods(false)}}

                        /// <summary>
                        /// Invoked whenever an instance is created allowed you to do some initializtion
                        /// </summary>
                        partial void OnCreate({{m_model.TypeName}} instance);
                    }
                }
                """;
        }

        private string RenderMethods(bool isInterface)
        {
            ClassWriter writer = new ClassWriter(
                    initialScope: 2,
                    indentSize: 4,
                    indentChar: ' ');

            foreach (var method in Iterator.Create(m_model.Methods))
            {
                RenderMethod(method, isInterface, writer);

                if (!method.IsLast) writer.WriteNewLine();
            }

            return writer.ToString();
        }



        private void RenderMethod(MethodModel model, bool isInterface, ClassWriter writer)
        {
            writer.WriteBlock($"""
                /// <summary>
                /// Creates a new instance of <see cref="{m_model.TypeName}"/>
                /// </summary>
                """);

            writer.WriteIf(!isInterface, "public ");
            writer.Write($"{m_model.TypeName} Create(");

            foreach (var parameter in Iterator.Create(model.Parameters))
            {
                WriteParameter(writer, parameter);
                if (!parameter.IsLast) writer.Write(", ");
            }

            // TODO: Params
            writer.Write(")");

            if (isInterface)
            {
                writer.Write(";");
                return;
            }
            writer.WriteNewLine();

            using (writer.StartScope())
            {
                writer.WriteLine("IParameter[] parameters = new IParameter[]");
                using (writer.StartScope(appendSemicolon: true))
                {
                    foreach (var parameter in Iterator.Create(model.Parameters))
                    {
                        writer.Write($"""new ConstructorArgument("{parameter.Value.Name}", {parameter.Value.Name})""");

                        if (parameter.IsLast)
                        {
                            writer.WriteNewLine();
                        }
                        else
                        {
                            writer.WriteLine(",");
                        }
                    }
                }
                writer.WriteLine($"""{m_model.TypeName} instance = m_resolutionRoot.Get<{m_model.TypeName}>(parameters);""");
                writer.WriteLine("OnCreate(instance);");
                writer.WriteLine("return instance;");
            }
        }

        private void WriteParameter(ClassWriter writer, ParameterModel parameter)
        {
            writer.Write($"{parameter.Type} {parameter.Name}");
        }
    }

}
