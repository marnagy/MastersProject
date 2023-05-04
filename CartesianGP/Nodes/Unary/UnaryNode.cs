public abstract class UnaryNode : CartesianNode
{
    protected UnaryNode(ParentIndices[] parents) : base(parents)
    {
        this.Arity = 1;
    }
}