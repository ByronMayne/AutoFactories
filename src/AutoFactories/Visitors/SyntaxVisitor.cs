namespace AutoFactories.Visitors
{
    internal abstract class SyntaxVisitor<T> : SyntaxVisitor
    {
        public void Accept(T syntax)
        {
            Visit(syntax);
        }

        protected abstract void Visit(T syntax);
    }
}