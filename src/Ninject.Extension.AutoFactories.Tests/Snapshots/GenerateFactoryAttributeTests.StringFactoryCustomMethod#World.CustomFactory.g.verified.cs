//HintName: World.CustomFactory.g.cs
#nullable enable
using System;
using Ninject;
using Ninject.Syntax;
using Ninject.Parameters;

namespace World
{
    public interface ICustomFactory 
    {
        /// <summary>
        /// Creates a new instance of <see cref="World.Person"/>
        /// </summary>
        global::World.Person CustomFunction();
    }
}

namespace World
{

    internal sealed partial class CustomFactory : global::World.ICustomFactory 
    {
        private readonly global::Ninject.Syntax.IResolutionRoot m_resolutionRoot;

        public CustomFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            m_resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of <see cref="World.Person"/>
        /// </summary>
        public global::World.Person CustomFunction()
        {
            IParameter[] parameters = new IParameter[]
            {
            };
            global::World.Person instance = m_resolutionRoot.Get<global::World.Person>(parameters);
            return instance;
        }
    }
}