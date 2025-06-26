using AutoFactories;

    public partial class ChairFactory 
    {}

    [AutoFactory(typeof(ChairFactory), "Chair", ExposeAs = typeof(object))]
    public class Chair
    {
        public Chair(string material)
        {}
    }
// -------------------------------------------------------------------
#nullable enable
#pragma warning disable CS8019 // Unnecessary using directive.

using AutoFactories;


    public partial class ChairFactory : IChairFactory
    {
        public ChairFactory()
        {
        }

        /// <summary>
        /// Creates a new instance of  <see cref="object"/>
        /// </summary>
        public global::object Chair(string material)
        {
            object __result = new global::Chair(
             material);
            return __result;
        }
    }