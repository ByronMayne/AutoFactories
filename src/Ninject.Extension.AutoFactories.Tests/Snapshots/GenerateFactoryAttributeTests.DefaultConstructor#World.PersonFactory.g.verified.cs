//HintName: World.PersonFactory.g.cs
#nullable enable
using System;
using Ninject;
using Ninject.Syntax;
using Ninject.Parameters;

namespace World
{
    public interface IPersonFactory 
    {
        /// <summary>
        /// Creates a new instance of <see cref="World.Person"/>
        /// </summary>
        global::World.Person Create();
    }
}

namespace World
{

    internal sealed partial class PersonFactory : global::World.IPersonFactory 
    {
        private readonly global::Ninject.Syntax.IResolutionRoot m_resolutionRoot;

        public PersonFactory(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
        {
            m_resolutionRoot = resolutionRoot;
        }

        /// <summary>
        /// Creates a new instance of <see cref="World.Person"/>
        /// </summary>
        public global::World.Person Create()
        {
            IParameter[] parameters = new IParameter[]
            {
            };
            global::World.Person instance = m_resolutionRoot.Get<global::World.Person>(parameters);
            return instance;
        }
    }
}