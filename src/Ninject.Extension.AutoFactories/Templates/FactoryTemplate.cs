using Ninject.AutoFactory.Models;
using Ninject.AutoFactory.Templates;
using Ninject.Extension.AutoFactories.Models;
using System.Net.Security;

namespace Ninject.Extension.AutoFactories.Templates
{
    internal class FactoryTemplate : Template
    {
        private readonly FactoryModel m_model;

        public FactoryTemplate(FactoryModel model) : base($"{model.Type.FullName}.g.cs")
        {
            m_model = model;
        }

        /// <inheritdoc cref="Template"/>
        /// <see cref=""/>
        protected override string Render()
        {
            return $$"""
                #nullable enable
                using System;
                using Ninject;
                using Ninject.Syntax;
                using Ninject.Parameters;
                
                namespace {{m_model.InterfaceType.Namespace}}
                {
                    {{m_model.InterfaceAccessModifier}} interface {{m_model.InterfaceType.TypeName}} 
                    {
                        {{RenderMethods(true)}}
                    }
                }

                namespace {{m_model.Type.Namespace}}
                {

                    {{m_model.AccessModifier}} sealed partial class {{m_model.Type.TypeName}}: global::{{m_model.InterfaceType.FullName}} 
                    {
                        private readonly global::Ninject.Syntax.IResolutionRoot m_resolutionRoot;

                        public {{m_model.Type.TypeName}}(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
                        {
                            m_resolutionRoot = resolutionRoot;
                        }

                        {{RenderMethods(false)}}
                    }
                }
                """;
        }

        private string RenderMethods(bool isInterface)
        {
            ClassWriter writer = new(
                    initialScope: 2,
                    indentSize: 4,
                    indentChar: ' ');

            foreach (var method in Iterator.Create(m_model.Products))
            {
                RenderProduct(method, isInterface, writer);

                if (!method.IsLast)
                {
                    writer.WriteNewLine();
                }
            }

            return writer.ToString();
        }

        private void RenderProduct(ProductModel product, bool isInterface, ClassWriter writer)
        {

            foreach (var constructor in Iterator.Create(product.Constructors))
            {
                writer.WriteBlock($"""
                /// <summary>
                /// Creates a new instance of <see cref="{product.ProductType.FullName}"/>
                /// </summary>
                """);

                writer.WriteIf(!isInterface, "public ");
                writer.Write($"global::{product.ProductType.FullName} {constructor.Value.Name}(");

                foreach (Iterator.Item<ParameterModel> parameter in Iterator.Create(constructor.Value.Parameters))
                {
                    WriteParameter(writer, parameter);
                    if (!parameter.IsLast)
                    {
                        writer.Write(", ");
                    }
                }
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
                        foreach (var parameter in Iterator.Create(constructor.Value.Parameters))
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
                    writer.WriteLine($"""global::{product.ProductType.FullName} instance = m_resolutionRoot.Get<global::{product.ProductType.FullName}>(parameters);""");
                    writer.WriteLine("return instance;");
                }

                if (!constructor.IsLast) writer.WriteNewLine();
            }
        }

        private void WriteParameter(ClassWriter writer, ParameterModel parameter)
        {
            writer.Write($"{parameter.Type} {parameter.Name}");
        }
    }

}
