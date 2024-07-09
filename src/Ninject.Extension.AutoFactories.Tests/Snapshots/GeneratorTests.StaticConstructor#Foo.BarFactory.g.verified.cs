//HintName: Foo.BarFactory.g.cs
#nullable enable
using System;
using Ninject;
using Ninject.Syntax;
using Ninject.Parameters;

namespace Foo
{
    public interface IBarFactory 
    {
        /// <summary>
        /// Creates a new instance of <see cref="Foo.Bar"/>
        /// </summary>
        global::Foo.Bar Create(string parameter);
    }
}

namespace Foo
{

    internal sealed partial class BarFactory : global::Foo.IBarFactory 
    {
        private readonly global::Ninject.Syntax.IResolutionRoot m_resolutionRoot;

        public BarFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            m_resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Foo.Bar"/>
        /// </summary>
        public global::Foo.Bar Create(string parameter)
        {
            IParameter[] parameters = new IParameter[]
            {
                new ConstructorArgument("parameter", parameter)
            };
            global::Foo.Bar instance = m_resolutionRoot.Get<global::Foo.Bar>(parameters);
            return instance;
        }
    }
}