namespace AutoFactories.CodeAnalysis
{
    internal enum ViewKind
    {
        None = 0,

        /// <summary>
        /// Views that are of fixed value and can't be changed. These are reserved for
        /// views that are embedded into AutoFactories 
        /// </summary>
        Fixed,

        /// <summary>
        /// A predefined type that would be applied multiple times
        /// </summary>
        Static,

        /// <summary>
        /// A partial class that is referenced by other pages of views
        /// </summary>
        Partial,

        /// <summary>
        /// A template that would be rendered a single time and has access to all the pages
        /// </summary>
        Template,
    }
}
