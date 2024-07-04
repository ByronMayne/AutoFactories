//HintName: REPLACED_Person_FACTORY.g.cs
#nullable enable
using System;
using Ninject;
using Ninject.Syntax;
using Ninject.Parameters;

namespace 
{
    public interface IREPLACED_Person_FACTORY 
    {
        /// <summary>
        /// Creates a new instance of <see cref="World.Person"/>
        /// </summary>
        global::World.Person Create();
    }
}

namespace 
{

    internal sealed partial class REPLACED_Person_FACTORY : global::IREPLACED_Person_FACTORY 
    {
        private readonly global::Ninject.Syntax.IResolutionRoot m_resolutionRoot;

        public REPLACED_Person_FACTORY(global::Ninject.Syntax.IResolutionRoot resolutionRoot)
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