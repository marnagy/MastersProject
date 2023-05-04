public abstract class BinaryNode : CartesianNode
{
    protected BinaryNode(ParentIndices[] parents) : base(parents)
    {
        this.Arity = 2;
    }
}