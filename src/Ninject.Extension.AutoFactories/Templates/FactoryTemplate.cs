using Ninject.AutoFactories.Models;

namespace Ninject.AutoFactories.Templates
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

            foreach (Iterator.Item<ProductModel> method in Iterator.Create(m_model.Products))
            {
                RenderProduct(method, isInterface, writer);

                if (!method.IsLast)
                {
                    _ = writer.WriteNewLine();
                }
            }

            return writer.ToString();
        }

        private void RenderProduct(ProductModel product, bool isInterface, ClassWriter writer)
        {

            foreach (Iterator.Item<ConstructorModel> constructor in Iterator.Create(product.Constructors))
            {
                _ = writer.WriteBlock($"""
                /// <summary>
                /// Creates a new instance of <see cref="{product.ProductType.FullName}"/>
                /// </summary>
                """);

                _ = writer.WriteIf(!isInterface, "public ");
                _ = writer.Write($"global::{product.ProductType.FullName} {constructor.Value.Name}(");

                foreach (Iterator.Item<ParameterModel> parameter in Iterator.Create(constructor.Value.Parameters))
                {
                    WriteParameter(writer, parameter);
                    if (!parameter.IsLast)
                    {
                        _ = writer.Write(", ");
                    }
                }
                _ = writer.Write(")");

                if (isInterface)
                {
                    _ = writer.Write(";");
                    return;
                }
                _ = writer.WriteNewLine();

                using (writer.StartScope())
                {
                    _ = writer.WriteLine("IParameter[] parameters = new IParameter[]");
                    using (writer.StartScope(appendSemicolon: true))
                    {
                        foreach (Iterator.Item<ParameterModel> parameter in Iterator.Create(constructor.Value.Parameters))
                        {
                            _ = writer.Write($"""new ConstructorArgument("{parameter.Value.Name}", {parameter.Value.Name})""");

                            _ = parameter.IsLast ? writer.WriteNewLine() : writer.WriteLine(",");
                        }
                    }
                    _ = writer.WriteLine($"""global::{product.ProductType.FullName} instance = m_resolutionRoot.Get<global::{product.ProductType.FullName}>(parameters);""");
                    _ = writer.WriteLine("return instance;");
                }

                if (!constructor.IsLast)
                {
                    _ = writer.WriteNewLine();
                }
            }
        }

        private void WriteParameter(ClassWriter writer, ParameterModel parameter)
        {
            _ = writer.Write($"{parameter.Type} {parameter.Name}");
        }
    }

}
